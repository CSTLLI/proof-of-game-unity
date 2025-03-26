using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
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
        
        private Dictionary<GameObject, bool> uiElementStates = new Dictionary<GameObject, bool>();
        
        public void Initialize(TextMeshProUGUI scenarioText, TextMeshProUGUI timerText, Slider progressBar, 
            string scenarioName, float totalTime)
        {
            this.scenarioNameText = scenarioText;
            this.timerText = timerText;
            this.progressBar = progressBar;
            this.scenarioName = scenarioName;
            this.totalTime = totalTime;
        
            SetScenarioName(scenarioName);
            remainingTime = totalTime;
            UpdateTimerDisplay();
            
            if (progressBar != null && progressBar.transform.parent != null)
            {
                percentText = progressBar.transform.parent.Find("PercentText")?.GetComponent<TextMeshProUGUI>();
            }
            
            UpdateProgress(0);
            
            isTimerRunning = false;
            
            Debug.Log("GameUIManager initialisé avec timer désactivé");
        }
        
        void Start()
        {
            scenarioManager = FindObjectOfType<ScenarioManager>();
            
            if (blockchainModeToggle != null)
            {
                blockchainModeToggle.onValueChanged.AddListener(OnBlockchainModeChanged);
                
                if (scenarioManager != null)
                {
                    blockchainModeToggle.isOn = scenarioManager.IsBlockchainModeEnabled();
                }
            }
            
            if (taskListButton != null)
            {
                taskListButton.onClick.AddListener(ToggleTaskList);
            }
            float aspectRatio = (float)Screen.width / Screen.height;
            isTabletMode = aspectRatio < 1.5f;
            
            if (isTabletMode)
            {
                AdjustForTabletMode();
            }
            
            if (displayHelpMessages)
            {
                nextHelpMessageTime = Time.time + 2f;
            }
            
            Debug.Log("GameUIManager démarré avec timer désactivé");
        }
        
        private void AdjustForTabletMode()
        {
            if (timerText != null)
            {
                timerText.fontSize += 2;
                
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
            
            if (progressBar != null)
            {
                RectTransform barRect = progressBar.GetComponent<RectTransform>();
                if (barRect != null)
                {
                    barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, barRect.sizeDelta.y + 5);
                }
            }
            
            if (scenarioNameText != null)
            {
                scenarioNameText.fontSize += 2;
            }
        }
        
        void Update()
        {
            if (isTimerRunning && remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
                
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    UpdateTimerDisplay();
                    isTimerRunning = false;
                    
                    if (OnTimeUp != null)
                    {
                        OnTimeUp.Invoke();
                    }
                }
            }
            
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
            
            string message = helpMessages[Random.Range(0, helpMessages.Length)];
            
            FeedbackUIController feedback = FindObjectOfType<FeedbackUIController>();
            if (feedback != null)
            {
                feedback.ShowTemporaryMessage(message, 5f);
            }
        }
        
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
                progressBar.value = progress;
                
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
                
                string message = isEnabled ? 
                    "Mode Blockchain activé. Sécurité renforcée." : 
                    "Mode Blockchain désactivé. Utilisez ce mode pour améliorer la sécurité.";
                
                Debug.Log(message);
                
                var feedbackUI = FindObjectOfType<FeedbackUIController>();
                if (feedbackUI != null)
                {
                    feedbackUI.ShowTemporaryMessage(message, 3f);
                }
            }
        }
        
        void ToggleTaskList()
        {
            TaskListUI taskList = FindObjectOfType<TaskListUI>();
            if (taskList != null)
            {
                taskList.ToggleTaskList();
            }
        }
        
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
        
        public void HideGameUI()
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.gameObject != gameObject)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void ShowGameUI()
        {
            if (uiElementStates.Count == 0)
            {
                Transform[] allChildren = GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.gameObject != gameObject)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                return;
            }
            
            foreach (var kvp in uiElementStates)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.SetActive(kvp.Value);
                }
            }
            
            uiElementStates.Clear();
        }
    }
}