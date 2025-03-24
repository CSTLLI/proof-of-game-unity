using UnityEngine;
using Interaction;

namespace Station.QRCode
{
    public class QRScannerStation : Interactable
    {
        public bool isBlockchainMode = false;
        public string savedInteractionText;

        public AudioClip scanStartSound;
        public AudioClip scanSuccessSound;
        public AudioClip scanErrorSound;

        [SerializeField] private QRScannerUIController uiController;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Start()
        {
            if (uiController == null)
            {
                uiController = FindObjectOfType<QRScannerUIController>();

                if (uiController == null)
                {
                    Debug.Log("QRScannerUIController non trouvé, tentative d'initialisation...");
                    InitializeUI();
                }
            }

            if (string.IsNullOrEmpty(savedInteractionText))
                savedInteractionText = interactionText;
        }

        private void InitializeUI()
        {
            QRScannerUIGenerator generator = FindObjectOfType<QRScannerUIGenerator>();

            if (generator == null)
            {
                // Créer un objet qui contiendra le générateur d'UI
                GameObject uiManagerObj = new GameObject("QRScannerManager");
                generator = uiManagerObj.AddComponent<QRScannerUIGenerator>();
                Debug.Log("Création d'un nouveau QRScannerUIGenerator");
            }

            // Générer l'interface UI
            generator.GenerateUI();

            // Trouver le controller qui a été créé
            uiController = FindObjectOfType<QRScannerUIController>();

            if (uiController == null)
                Debug.LogError("Échec de création du QRScannerUIController");
            else
                Debug.Log("QRScannerUIController initialisé avec succès");
        }

        public override void Interact()
        {
            // Enregistrer le texte d'interaction
            if (string.IsNullOrEmpty(savedInteractionText))
                savedInteractionText = interactionText;

            // Vider temporairement le texte d'interaction
            interactionText = "";

            // Jouer le son de démarrage
            if (scanStartSound != null)
                audioSource.PlayOneShot(scanStartSound);

            // Chercher l'UIController si on ne l'a pas déjà
            if (uiController == null)
                uiController = FindObjectOfType<QRScannerUIController>();

            if (uiController != null)
            {
                // IMPORTANT: Utiliser le PlayerInteractionController pour obtenir l'aileron actuellement ciblé
                PlayerInteractionController playerController = FindObjectOfType<PlayerInteractionController>();
                if (playerController != null && playerController.currentInteractable != null)
                {
                    // Trouver le WingIdentifier attaché à cet interactable ou à son parent
                    WingIdentifier targetWing = playerController.currentInteractable.GetComponent<WingIdentifier>();

                    if (targetWing == null && playerController.currentInteractable.transform.parent != null)
                    {
                        targetWing =
                            playerController.currentInteractable.transform.parent.GetComponent<WingIdentifier>();
                    }

                    if (targetWing != null)
                    {
                        string aileronId = targetWing.GetAileronId();
                        Debug.Log(
                            $"Scanner interagit avec l'aileron ciblé: {targetWing.GetDisplayName()}, ID: {aileronId}");
                        uiController.StartScan(this, aileronId);
                        return;
                    }
                }

                // Si on ne trouve pas l'aileron via l'interaction controller, essayer le raycast
                Camera playerCamera = Camera.main;
                if (playerCamera != null)
                {
                    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                    RaycastHit hit;

                    Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

                    if (Physics.Raycast(ray, out hit, 10f))
                    {
                        WingIdentifier wing = hit.collider.GetComponent<WingIdentifier>();
                        if (wing == null && hit.transform.parent != null)
                        {
                            wing = hit.transform.parent.GetComponent<WingIdentifier>();
                        }

                        if (wing != null)
                        {
                            string aileronId = wing.GetAileronId();
                            Debug.Log(
                                $"Scanner interagit avec l'aileron visé: {wing.GetDisplayName()}, ID: {aileronId}");
                            uiController.StartScan(this, aileronId);
                            return;
                        }
                    }
                }

                // Si aucune des méthodes ci-dessus ne fonctionne, utiliser la méthode de secours
                // qui cherche dans tous les gameobjects de la scène
                GameObject[] wingObjects = GameObject.FindGameObjectsWithTag("Wing");
                foreach (GameObject obj in wingObjects)
                {
                    string objectName = obj.name;
                    Debug.Log($"Recherche d'ailerons - Trouvé: {objectName}");

                    WingIdentifier wing = obj.GetComponent<WingIdentifier>();
                    if (wing != null)
                    {
                        string aileronId = wing.GetAileronId();
                        Debug.Log(
                            $"Dernier recours - Scanner interagit avec: {wing.GetDisplayName()}, ID: {aileronId}");
                        uiController.StartScan(this, aileronId);
                        return;
                    }
                }

                // Si on arrive ici, c'est qu'aucun aileron n'a été trouvé
                Debug.LogWarning("Aucun aileron trouvé dans la scène!");
                uiController.StartScan(this, "monaco1"); // ID par défaut
            }
            else
            {
                Debug.LogError("Impossible de trouver un QRScannerUIController dans la scène");
                interactionText = savedInteractionText;
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
    }
}