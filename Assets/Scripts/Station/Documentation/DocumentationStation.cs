using UnityEngine;
using System.Collections;
using Interaction;

namespace Station.Documentation
{
    public class DocumentationStation : Interactable
    {
        public bool isBlockchainMode = false;
        public string savedInteractionText;
        
        public AudioClip startSound;
        public AudioClip successSound;
        public AudioClip errorSound;
        
        [SerializeField] private DocumentationUIController uiController;
        [SerializeField] public string associatedTaskName = "verify_docs_aileron";
        
        private AudioSource audioSource;
        private static bool uiInitialized = false;
        
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
                uiController = FindObjectOfType<DocumentationUIController>();
            }
        }
        
        private void InitializeUI()
        {
            DocumentationUIGenerator generator = FindObjectOfType<DocumentationUIGenerator>();
            
            if (generator == null)
            {
                GameObject managerObj = GameObject.Find("DocumentationManager");
                if (managerObj == null)
                    managerObj = new GameObject("DocumentationManager");
                    
                generator = managerObj.AddComponent<DocumentationUIGenerator>();
            }
            
            generator.GenerateUI();
            uiController = FindObjectOfType<DocumentationUIController>();
            
            if (uiController == null)
                Debug.LogError("Impossible de créer ou trouver le DocumentationUIController.");
            else
                uiController.SetAssociatedTaskName(associatedTaskName);
        }
        
        public override void Interact()
        {
            savedInteractionText = interactionText;
            interactionText = "";
            
            if (startSound != null)
                audioSource.PlayOneShot(startSound);
                
            if (uiController == null)
            {
                uiController = FindObjectOfType<DocumentationUIController>();
                
                if (uiController == null)
                {
                    Debug.LogWarning("DocumentationUIController introuvable. Tentative de réinitialisation...");
                    InitializeUI();
                }
            }
            
            if (uiController != null)
            {
                uiController.StartDocumentation(this);
                uiController.SetAssociatedTaskName(associatedTaskName);
            }
            else
            {
                Debug.LogError("Impossible de trouver DocumentationUIController. Processus de secours...");
                StartCoroutine(FallbackProcess());
                interactionText = savedInteractionText;
            }
        }
        
        private IEnumerator FallbackProcess()
        {
            yield return new WaitForSeconds(1.5f);
            
            if (isBlockchainMode)
            {
                Debug.Log("Mode Blockchain: Vérification instantanée réussie!");
                
                if (successSound != null)
                    audioSource.PlayOneShot(successSound);
            }
            else
            {
                Debug.Log("Mode Standard: Vérification manuelle requise...");
                
                if (Random.value < 0.3f)
                {
                    Debug.Log("Erreur de saisie détectée!");
                    
                    if (errorSound != null)
                        audioSource.PlayOneShot(errorSound);
                }
                else
                {
                    Debug.Log("Vérification manuelle terminée.");
                    
                    if (successSound != null)
                        audioSource.PlayOneShot(successSound);
                }
            }
        }
        
        public void PlaySuccessSound()
        {
            if (successSound != null)
                audioSource.PlayOneShot(successSound);
        }
        
        public void PlayErrorSound()
        {
            if (errorSound != null)
                audioSource.PlayOneShot(errorSound);
        }
    }
}