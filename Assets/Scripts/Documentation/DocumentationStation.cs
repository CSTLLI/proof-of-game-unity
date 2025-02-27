using UnityEngine;
using System.Collections;

public class DocumentationStation : Interactable
{
    public bool isBlockchainMode = false;
    
    public string savedInteractionText;
    
    public AudioClip startSound;
    public AudioClip successSound;
    public AudioClip errorSound;
    
    // Référence au contrôleur UI
    [SerializeField] private DocumentationUIController uiController;
    
    // Composant AudioSource
    private AudioSource audioSource;
    
    private static bool uiInitialized = false;
    public string associatedTaskName = "verify_docs_aileron";
    
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
                uiController = FindObjectOfType<DocumentationUIController>();
            }
        }
    }
    
    private void InitializeUI()
    {
        // Chercher un générateur existant
        DocumentationUIGenerator generator = FindObjectOfType<DocumentationUIGenerator>();
        
        // Si aucun générateur n'existe, en créer un
        if (generator == null)
        {
            GameObject managerObj = GameObject.Find("DocumentationManager");
            if (managerObj == null)
            {
                managerObj = new GameObject("DocumentationManager");
            }
            generator = managerObj.AddComponent<DocumentationUIGenerator>();
        }
        
        // Générer l'UI
        generator.GenerateUI();
        
        // Récupérer le contrôleur
        uiController = FindObjectOfType<DocumentationUIController>();
        
        if (uiController == null)
        {
            Debug.LogError("Impossible de créer ou trouver le DocumentationUIController.");
        }
        else
        {
            uiController.SetAssociatedTaskName(associatedTaskName);
        }
    }
    
    public override void Interact()
    {
        Debug.Log("Interaction avec la station de documentation");
    
        // Sauvegarder et masquer le texte d'interaction
        savedInteractionText = interactionText;
        interactionText = "";
        
        // Jouer le son de début d'interaction
        if (startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }
        
        // Si le contrôleur n'est toujours pas disponible, réessayer l'initialisation
        if (uiController == null)
        {
            uiController = FindObjectOfType<DocumentationUIController>();
            
            if (uiController == null)
            {
                Debug.LogWarning("DocumentationUIController introuvable. Tentative de réinitialisation...");
                InitializeUI();
            }
        }
    
        // Démarrer l'interaction avec l'UI si disponible
        if (uiController != null)
        {
            uiController.StartDocumentation(this);
            uiController.SetAssociatedTaskName(associatedTaskName);
        }
        else
        {
            Debug.LogError("Impossible de trouver ou créer DocumentationUIController. Utilisation du processus de secours.");
            StartCoroutine(FallbackProcess());
            
            // Restaurer le texte d'interaction puisque l'UI n'est pas disponible
            interactionText = savedInteractionText;
        }
    }
    
    private IEnumerator FallbackProcess()
    {
        // Attendre un court délai pour simuler le traitement
        yield return new WaitForSeconds(1.5f);
        
        if (isBlockchainMode)
        {
            Debug.Log("Mode Blockchain: Vérification instantanée réussie!");
            Debug.Log("Données auto-remplies et vérifiées sur la blockchain");
            
            // Jouer le son de succès
            if (successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
        }
        else
        {
            Debug.Log("Mode Standard: Vérification manuelle requise...");
            
            // Simuler un délai et une possibilité d'erreur
            if (Random.value < 0.3f)
            {
                Debug.Log("Erreur de saisie détectée! Vérifiez les données.");
                
                // Jouer le son d'erreur
                if (errorSound != null)
                {
                    audioSource.PlayOneShot(errorSound);
                }
            }
            else
            {
                Debug.Log("Vérification manuelle terminée, données enregistrées.");
                
                // Jouer le son de succès
                if (successSound != null)
                {
                    audioSource.PlayOneShot(successSound);
                }
            }
        }
    }
    
    public void PlaySuccessSound()
    {
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }
    
    public void PlayErrorSound()
    {
        if (errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }
}