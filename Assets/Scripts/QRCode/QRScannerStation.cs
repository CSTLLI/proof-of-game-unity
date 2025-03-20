using UnityEngine;
using System.Collections;

public class QRScannerStation : Interactable
{
    public bool isBlockchainMode = false;
    
    public string savedInteractionText;
    
    public AudioClip scanStartSound;
    public AudioClip scanSuccessSound;
    public AudioClip scanErrorSound;
    
    // Référence au contrôleur UI
    [SerializeField] private QRScannerUIController uiController;
    [SerializeField] private string associatedTaskName = "scanner_admission";
    
    // Composant AudioSource
    private AudioSource audioSource;
    
    private static bool uiInitialized = false;
    
    private void Awake()
    {
        // Configuration de l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void Start()
    {
        // N'initialiser l'UI qu'une seule fois, pour toutes les stations
        if (!uiInitialized)
        {
            InitializeUI();
            uiInitialized = true;
        }
        else
        {
            // Si l'UI est déjà initialisée, trouvez simplement le contrôleur
            if (uiController == null)
            {
                uiController = FindObjectOfType<QRScannerUIController>();
            }
        }
    }
    
    private void InitializeUI()
    {
        // Chercher un générateur existant
        QRScannerUIGenerator generator = FindObjectOfType<QRScannerUIGenerator>();
        
        // Si aucun générateur n'existe, en créer un
        if (generator == null)
        {
            GameObject managerObj = GameObject.Find("QRScannerManager");
            if (managerObj == null)
            {
                managerObj = new GameObject("QRScannerManager");
            }
            generator = managerObj.AddComponent<QRScannerUIGenerator>();
        }
        
        // Générer l'UI
        generator.GenerateUI();
        
        // Récupérer le contrôleur
        uiController = FindObjectOfType<QRScannerUIController>();
        
        if (uiController == null)
        {
            Debug.LogError("Impossible de créer ou trouver le QRScannerUIController.");
        }
    }
    
    public override void Interact()
    {
        Debug.Log("Interaction avec la station scanner QR");

        // Sauvegarder et masquer le texte d'interaction
        savedInteractionText = interactionText;
        interactionText = "";
    
        // Jouer le son de début de scan
        if (scanStartSound != null)
        {
            audioSource.PlayOneShot(scanStartSound);
        }
    
        // Si le contrôleur n'est toujours pas disponible, réessayer l'initialisation
        if (uiController == null)
        {
            uiController = FindObjectOfType<QRScannerUIController>();
        
            if (uiController == null)
            {
                Debug.LogWarning("QRScannerUIController introuvable. Tentative de réinitialisation...");
                // InitializeUI();
            }
        }

        // Démarrer le scan avec l'UI si disponible
        if (uiController != null)
        {
            // Passer le nom de la tâche associée à cette station
            uiController.StartScan(this, associatedTaskName);
        }
        else
        {
            Debug.LogError("Impossible de trouver ou créer QRScannerUIController. Utilisation du processus de scan de secours.");
            StartCoroutine(ScanProcess());
        
            // Restaurer le texte d'interaction puisque l'UI n'est pas disponible
            interactionText = savedInteractionText;
        }
    }
    
    private IEnumerator ScanProcess()
    {
        // Attendre un court délai pour simuler le scan
        yield return new WaitForSeconds(1.5f);
        
        if (isBlockchainMode)
        {
            Debug.Log("Mode Blockchain: Scan instantané réussi!");
            Debug.Log("Données de traçabilité validées sur la blockchain");
            
            // Jouer le son de succès
            if (scanSuccessSound != null)
            {
                audioSource.PlayOneShot(scanSuccessSound);
            }
        }
        else
        {
            Debug.Log("Mode Standard: Scan en cours...");
            
            // Simuler un délai et une possibilité d'erreur
            if (Random.value < 0.4f)
            {
                Debug.Log("Erreur de vérification! Risque augmenté.");
                
                // Jouer le son d'erreur
                if (scanErrorSound != null)
                {
                    audioSource.PlayOneShot(scanErrorSound);
                }
            }
            else
            {
                Debug.Log("Vérification terminée, données limitées disponibles.");
                
                // Jouer le son de succès
                if (scanSuccessSound != null)
                {
                    audioSource.PlayOneShot(scanSuccessSound);
                }
            }
        }
    }
    
    public void PlaySuccessSound()
    {
        if (scanSuccessSound != null)
        {
            audioSource.PlayOneShot(scanSuccessSound);
        }
    }
    
    public void PlayErrorSound()
    {
        if (scanErrorSound != null)
        {
            audioSource.PlayOneShot(scanErrorSound);
        }
    }
}