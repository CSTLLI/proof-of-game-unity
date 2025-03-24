using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Core;
using UI.Feedback;
using Random = UnityEngine.Random;

namespace UI.UICore
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Scenario Info")]
        [SerializeField] private TextMeshProUGUI scenarioNameText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Slider progressBar;
        
        [Header("Control Panel")]
        [SerializeField] private Toggle blockchainModeToggle;
        [SerializeField] private Button taskListButton;
        
        // Référence au texte de pourcentage pour la barre de progression
        private TextMeshProUGUI percentText;
        
        // Événement déclenché lorsque le temps est écoulé
        public event Action OnTimeUp;
        
        // Référence au ScenarioManager
        private ScenarioManager scenarioManager;
        
        // Variables pour le timer
        private string scenarioName;
        private float totalTime;
        private float remainingTime;
        private bool isTimerRunning = false;
        
        // Variables pour l'interface responsive
        private bool isTabletMode = false;
        [SerializeField] private bool displayHelpMessages = true;
        private float nextHelpMessageTime = 0f;
        
        public void Initialize(TextMeshProUGUI scenarioText, TextMeshProUGUI timerText, Slider progressBar, 
            string scenarioName, float totalTime)
        {
            this.scenarioNameText = scenarioText;
            this.timerText = timerText;
            this.progressBar = progressBar;
            this.scenarioName = scenarioName;
            this.totalTime = totalTime;
        
            // Configuration initiale
            SetScenarioName(scenarioName);
            remainingTime = totalTime;
            UpdateTimerDisplay();
            
            // Trouver le texte de pourcentage
            if (progressBar != null && progressBar.transform.parent != null)
            {
                percentText = progressBar.transform.parent.Find("PercentText")?.GetComponent<TextMeshProUGUI>();
            }
            
            // Initialiser la progression à 0%
            UpdateProgress(0);
            
            // Timer désactivé par défaut
            isTimerRunning = false;
            
            Debug.Log("GameUIManager initialisé avec timer désactivé");
        }
        
        void Start()
        {
            // Trouver le ScenarioManager
            scenarioManager = FindObjectOfType<ScenarioManager>();
            
            // Configurer les événements UI
            if (blockchainModeToggle != null)
            {
                blockchainModeToggle.onValueChanged.AddListener(OnBlockchainModeChanged);
                
                // Initialiser le toggle avec la valeur actuelle
                if (scenarioManager != null)
                {
                    blockchainModeToggle.isOn = scenarioManager.IsBlockchainModeEnabled();
                }
            }
            
            if (taskListButton != null)
            {
                taskListButton.onClick.AddListener(ToggleTaskList);
            }
            
            // Détecter si on est sur tablette
            float aspectRatio = (float)Screen.width / Screen.height;
            isTabletMode = aspectRatio < 1.5f;
            
            // Ajuster l'interface pour tablette
            if (isTabletMode)
            {
                AdjustForTabletMode();
            }
            
            // Programmer l'affichage du premier message d'aide
            if (displayHelpMessages)
            {
                nextHelpMessageTime = Time.time + 2f;
            }
            
            Debug.Log("GameUIManager démarré avec timer désactivé");
        }
        
        private void AdjustForTabletMode()
        {
            // Ajuster l'échelle du timer pour qu'il soit plus lisible
            if (timerText != null)
            {
                timerText.fontSize += 2;
                
                // Agrandir le conteneur du timer si disponible
                Transform timerPanel = timerText.transform.parent;
                if (timerPanel != null)
                {
                    RectTransform panelRect = timerPanel.GetComponent<RectTransform>();
                    if (panelRect != null)
                    {
                        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x + 20, panelRect.sizeDelta.y + 10);
                    }
                }
            }
            
            // Ajuster la barre de progression
            if (progressBar != null)
            {
                RectTransform barRect = progressBar.GetComponent<RectTransform>();
                if (barRect != null)
                {
                    // Agrandir légèrement pour une meilleure visibilité
                    barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, barRect.sizeDelta.y + 5);
                }
            }
            
            // Ajuster le nom du scénario
            if (scenarioNameText != null)
            {
                scenarioNameText.fontSize += 2;
            }
        }
        
        void Update()
        {
            // Mise à jour du timer
            if (isTimerRunning && remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
                
                // Vérifier si le temps est écoulé
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    UpdateTimerDisplay();
                    isTimerRunning = false;
                    
                    // Déclencher l'événement de fin de temps
                    if (OnTimeUp != null)
                    {
                        OnTimeUp.Invoke();
                    }
                }
            }
            
            // Afficher des messages d'aide périodiques
            if (displayHelpMessages && isTimerRunning && Time.time > nextHelpMessageTime)
            {
                ShowRandomHelpMessage();
                nextHelpMessageTime = Time.time + Random.Range(60f, 120f); // Messages tous les 1-2 minutes
            }
        }
        
        private void ShowRandomHelpMessage()
        {
            string[] helpMessages = new string[]
            {
                "Appuyez sur [Tab] pour voir la liste des tâches.",
                "Le mode Blockchain offre une sécurité renforcée pour la vérification des pièces.",
                "Consultez l'indicateur de tâche actuelle en haut à gauche de l'écran.",
                "Le temps restant est affiché en haut à droite. Gérez votre temps efficacement.",
                "Certaines tâches nécessitent le mode Blockchain. Activez-le pour ces tâches spécifiques."
            };
            
            // Choisir un message aléatoire
            string message = helpMessages[Random.Range(0, helpMessages.Length)];
            
            // Afficher via le FeedbackUIController
            FeedbackUIController feedback = FindObjectOfType<FeedbackUIController>();
            if (feedback != null)
            {
                feedback.ShowTemporaryMessage(message, 5f);
            }
        }
        
        // Méthode pour afficher/masquer le panneau des tâches
        public void ShowTasks(bool show)
        {
            Transform taskPanel = transform.Find("TaskPanel");
            if (taskPanel != null)
            {
                taskPanel.gameObject.SetActive(show);
            }
        }
        
        public void SetScenarioName(string name)
        {
            if (scenarioNameText != null)
            {
                scenarioNameText.text = name;
            }
        }
        
        public void ResetTimer(float duration)
        {
            remainingTime = duration;
            UpdateTimerDisplay();
            
            Debug.Log($"Timer réinitialisé à {duration} secondes");
        }
        
        void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                // Convertir en minutes:secondes
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                // Changer la couleur en fonction du temps restant
                if (remainingTime < 60) // moins d'une minute
                    timerText.color = Color.red;
                else if (remainingTime < 180) // moins de 3 minutes
                    timerText.color = Color.yellow;
                else
                    timerText.color = Color.white;
            }
        }
        
        public void UpdateProgress(float progress)
        {
            if (progressBar != null)
            {
                // Mettre à jour la valeur du slider
                progressBar.value = progress;
                
                // Mettre à jour directement le texte de pourcentage
                if (percentText == null && progressBar.transform.parent != null)
                {
                    percentText = progressBar.transform.parent.Find("PercentText")?.GetComponent<TextMeshProUGUI>();
                }
                
                if (percentText != null)
                {
                    int percentage = Mathf.RoundToInt(progress * 100);
                    percentText.text = percentage + "%";
                    Debug.Log($"Barre de progression mise à jour: {percentage}%");
                }
                else
                {
                    Debug.LogWarning("TextMeshProUGUI pour le pourcentage non trouvé");
                }
            }
            else
            {
                Debug.LogWarning("ProgressBar est null");
            }
        }
        
        void OnBlockchainModeChanged(bool isEnabled)
        {
            if (scenarioManager != null)
            {
                scenarioManager.SetBlockchainMode(isEnabled);
                
                // Afficher un message pour indiquer le changement de mode
                string message = isEnabled ? 
                    "Mode Blockchain activé. Sécurité renforcée." : 
                    "Mode Blockchain désactivé. Utilisez ce mode pour améliorer la sécurité.";
                
                Debug.Log(message);
                
                // Afficher un message temporaire à l'écran
                var feedbackUI = FindObjectOfType<FeedbackUIController>();
                if (feedbackUI != null)
                {
                    feedbackUI.ShowTemporaryMessage(message, 3f);
                }
            }
        }
        
        void ToggleTaskList()
        {
            // Trouver et afficher/masquer la liste des tâches
            TaskListUI taskList = FindObjectOfType<TaskListUI>();
            if (taskList != null)
            {
                taskList.ToggleTaskList();
            }
        }
        
        // Méthode pour démarrer/arrêter le timer
        public void ToggleTimer(bool enable)
        {
            isTimerRunning = enable;
            
            if (enable)
            {
                Debug.Log("Timer démarré - temps restant : " + remainingTime);
            }
            else
            {
                Debug.Log("Timer arrêté - temps restant : " + remainingTime);
            }
        }
        
        public float GetRemainingTime()
        {
            return remainingTime;
        }
        
        // Méthode pour vérifier si on est en mode tablette
        public bool IsTabletMode()
        {
            return isTabletMode;
        }
    }
}