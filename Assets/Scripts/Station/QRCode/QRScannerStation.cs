using UnityEngine;
using System.Collections;
using Core;
using Interaction;
using UI.Feedback;

namespace Station.QRCode
{
    public class QRScannerStation : Interactable
    {
        public bool isBlockchainMode = false;
        public string savedInteractionText;
        
        public AudioClip scanStartSound;
        public AudioClip scanSuccessSound;
        public AudioClip scanErrorSound;
        
        [Header("Scanner Configuration")]
        [SerializeField] private Transform scanPoint; // Point où l'aileron doit être placé pour être scanné
        [SerializeField] private float scanRadius = 3f; // Rayon de détection des ailerons
        [SerializeField] private QRScannerUIController uiController;
        
        private AudioSource audioSource;
        private static bool uiInitialized = false;
        
        private WingIdentifier detectedAileron;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        private void Start()
        {
            if (!uiInitialized)
            {
                InitializeUI();
                uiInitialized = true;
            }
            else if (uiController == null)
            {
                uiController = FindObjectOfType<QRScannerUIController>();
            }
            
            if (scanPoint == null)
                scanPoint = transform;
        }
        
        private void Update()
        {
            DetectNearbyAileron();
            
            if (detectedAileron != null && interactionText == savedInteractionText)
            {
                interactionText = $"Scanner l'aileron {detectedAileron.GetDisplayName()}";
            }
            else if (detectedAileron == null && interactionText != savedInteractionText && !string.IsNullOrEmpty(savedInteractionText))
            {
                interactionText = savedInteractionText;
            }
        }
        
        private void DetectNearbyAileron()
        {
            Collider[] colliders = Physics.OverlapSphere(scanPoint.position, scanRadius, ~0);
            Debug.Log($"Found {colliders.Length} colliders in scan radius");
    
            // Afficher des informations sur chaque collider
            foreach (Collider col in colliders)
            {
                // Debug.Log($"Found collider: {col.name}, Tag: {col.tag}, Has WingIdentifier: {col.GetComponent<WingIdentifier>() != null}");
            }
    
            WingIdentifier closestAileron = null;
            float closestDistance = scanRadius;
    
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Wing"))
                {
                    WingIdentifier aileron = col.GetComponent<WingIdentifier>();
                    if (aileron != null)
                    {
                        float dist = Vector3.Distance(scanPoint.position, col.transform.position);
                        Debug.Log($"Found wing: {col.name}, Distance: {dist}");
                
                        if (dist < closestDistance)
                        {
                            closestAileron = aileron;
                            closestDistance = dist;
                        }
                    }
                    else
                    {
                        Debug.Log($"Object {col.name} has Wing tag but no WingIdentifier component");
                    }
                }
            }
    
            detectedAileron = closestAileron;
            Debug.Log($"Closest wing detected: {(detectedAileron != null ? detectedAileron.GetDisplayName() : "None")}");
        }
        
        private void InitializeUI()
        {
            QRScannerUIGenerator generator = FindObjectOfType<QRScannerUIGenerator>();
            
            if (generator == null)
            {
                GameObject managerObj = GameObject.Find("QRScannerManager");
                if (managerObj == null)
                    managerObj = new GameObject("QRScannerManager");
                    
                generator = managerObj.AddComponent<QRScannerUIGenerator>();
            }
            
            generator.GenerateUI();
            uiController = FindObjectOfType<QRScannerUIController>();
            
            if (uiController == null)
                Debug.LogError("Impossible de créer ou trouver le QRScannerUIController.");
        }
        
        public override void Interact()
        {
            DetectNearbyAileron();
            
            // Vérifier si un aileron est détecté
            if (detectedAileron == null)
            {
                Debug.Log("Aucun aileron détecté à proximité du scanner");
                if (feedbackController != null)
                    feedbackController.ShowTemporaryMessage("Aucun aileron à scanner. Rapprochez un aileron de la station.", 3f);
                return;
            }
            
            savedInteractionText = interactionText;
            interactionText = "";
            
            if (scanStartSound != null)
                audioSource.PlayOneShot(scanStartSound);
                
            if (uiController == null)
            {
                uiController = FindObjectOfType<QRScannerUIController>();
                
                if (uiController == null)
                {
                    Debug.LogWarning("QRScannerUIController introuvable. Tentative de réinitialisation...");
                    InitializeUI();
                }
            }
            
            if (uiController != null)
                uiController.StartScan(this, detectedAileron.GetAileronId());
            else
            {
                Debug.LogError("Impossible de trouver QRScannerUIController. Processus de secours...");
                StartCoroutine(ScanProcess(detectedAileron.GetAileronId()));
                interactionText = savedInteractionText;
            }
        }
        
        private IEnumerator ScanProcess(string aileronId)
        {
            yield return new WaitForSeconds(1.5f);
            
            // Trouver le ScenarioManager pour valider l'aileron
            ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
            
            if (isBlockchainMode)
            {
                Debug.Log("Mode Blockchain: Scan instantané réussi!");
                
                if (scanSuccessSound != null)
                    audioSource.PlayOneShot(scanSuccessSound);
                    
                if (scenarioManager != null)
                    scenarioManager.ValidateAileron(aileronId);
            }
            else
            {
                Debug.Log("Mode Standard: Scan en cours...");
                
                if (Random.value < 0.4f)
                {
                    Debug.Log("Erreur de vérification!");
                    
                    if (scanErrorSound != null)
                        audioSource.PlayOneShot(scanErrorSound);
                }
                else
                {
                    Debug.Log("Vérification terminée.");
                    
                    if (scanSuccessSound != null)
                        audioSource.PlayOneShot(scanSuccessSound);
                        
                    if (scenarioManager != null)
                        scenarioManager.ValidateAileron(aileronId);
                }
            }
        }
        
        public void PlaySuccessSound()
        {
            if (scanSuccessSound != null)
                audioSource.PlayOneShot(scanSuccessSound);
        }
        
        public void PlayErrorSound()
        {
            if (scanErrorSound != null)
                audioSource.PlayOneShot(scanErrorSound);
        }
        
        private FeedbackUIController feedbackController;
        
        private void OnDrawGizmosSelected()
        {
            // Dessiner le rayon de détection dans l'éditeur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(scanPoint ? scanPoint.position : transform.position, scanRadius);
        }
    }
}