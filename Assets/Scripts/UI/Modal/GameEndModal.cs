using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace UI.Modal
{
    public class GameEndModal : MonoBehaviour
    {
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI resultsText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI riskText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        
        private ScenarioManager scenarioManager;

        void Start()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClicked);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
                
            if (modalPanel != null)
                modalPanel.SetActive(false);
            
            scenarioManager = FindFirstObjectByType<ScenarioManager>();
        }

        public void CreateModalUI()
        {
            // Créer le panel principal
            modalPanel = new GameObject("EndModalPanel");
            modalPanel.transform.SetParent(transform, false);

            // Ajouter Canvas
            Canvas canvas = modalPanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            modalPanel.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            modalPanel.AddComponent<GraphicRaycaster>();

            // Créer le fond
            GameObject background = new GameObject("Background");
            background.transform.SetParent(modalPanel.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);

            // Créer le contenu
            GameObject content = new GameObject("Content");
            content.transform.SetParent(modalPanel.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(600, 400);
            contentRect.anchoredPosition = Vector2.zero;
            Image contentImage = content.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Créer le titre
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(content.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 60);
            titleRect.anchoredPosition = new Vector2(0, -30);
            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Résultat de la Mission";
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;

            // Créer le texte des résultats
            GameObject resultsObj = new GameObject("Results");
            resultsObj.transform.SetParent(content.transform, false);
            RectTransform resultsRect = resultsObj.AddComponent<RectTransform>();
            resultsRect.anchorMin = new Vector2(0, 0);
            resultsRect.anchorMax = new Vector2(1, 1);
            resultsRect.sizeDelta = new Vector2(-40, -120);
            resultsRect.anchoredPosition = new Vector2(0, -20);
            resultsText = resultsObj.AddComponent<TextMeshProUGUI>();
            resultsText.text = "Résultats de la mission";
            resultsText.fontSize = 18;
            resultsText.alignment = TextAlignmentOptions.Center;
            resultsText.color = Color.white;

            // Créer le texte du temps
            GameObject timeObj = new GameObject("TimeText");
            timeObj.transform.SetParent(content.transform, false);
            RectTransform timeRect = timeObj.AddComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0, 0);
            timeRect.anchorMax = new Vector2(1, 1);
            timeRect.sizeDelta = new Vector2(-40, -200);
            timeRect.anchoredPosition = new Vector2(0, 0);
            timeText = timeObj.AddComponent<TextMeshProUGUI>();
            timeText.text = "Temps écoulé";
            timeText.fontSize = 16;
            timeText.alignment = TextAlignmentOptions.Center;
            timeText.color = Color.white;

            // Créer le texte du risque
            GameObject riskObj = new GameObject("RiskText");
            riskObj.transform.SetParent(content.transform, false);
            RectTransform riskRect = riskObj.AddComponent<RectTransform>();
            riskRect.anchorMin = new Vector2(0, 0);
            riskRect.anchorMax = new Vector2(1, 1);
            riskRect.sizeDelta = new Vector2(-40, -240);
            riskRect.anchoredPosition = new Vector2(0, 20);
            riskText = riskObj.AddComponent<TextMeshProUGUI>();
            riskText.text = "Niveau de risque";
            riskText.fontSize = 16;
            riskText.alignment = TextAlignmentOptions.Center;
            riskText.color = Color.white;

            // Créer les boutons
            GameObject restartButtonObj = new GameObject("RestartButton");
            restartButtonObj.transform.SetParent(content.transform, false);
            RectTransform restartButtonRect = restartButtonObj.AddComponent<RectTransform>();
            restartButtonRect.anchorMin = new Vector2(0.5f, 0);
            restartButtonRect.anchorMax = new Vector2(0.5f, 0);
            restartButtonRect.sizeDelta = new Vector2(200, 50);
            restartButtonRect.anchoredPosition = new Vector2(-110, 40);
            Image restartButtonImage = restartButtonObj.AddComponent<Image>();
            restartButtonImage.color = new Color(0.2f, 0.6f, 0.2f);
            restartButton = restartButtonObj.AddComponent<Button>();
            restartButton.targetGraphic = restartButtonImage;
            restartButton.interactable = true;

            GameObject restartButtonTextObj = new GameObject("RestartButtonText");
            restartButtonTextObj.transform.SetParent(restartButtonObj.transform, false);
            RectTransform restartButtonTextRect = restartButtonTextObj.AddComponent<RectTransform>();
            restartButtonTextRect.anchorMin = Vector2.zero;
            restartButtonTextRect.anchorMax = Vector2.one;
            restartButtonTextRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI restartButtonText = restartButtonTextObj.AddComponent<TextMeshProUGUI>();
            restartButtonText.text = "Recommencer";
            restartButtonText.fontSize = 16;
            restartButtonText.alignment = TextAlignmentOptions.Center;
            restartButtonText.color = Color.white;

            // Bouton Quitter
            GameObject quitButtonObj = new GameObject("QuitButton");
            quitButtonObj.transform.SetParent(content.transform, false);
            RectTransform quitButtonRect = quitButtonObj.AddComponent<RectTransform>();
            quitButtonRect.anchorMin = new Vector2(0.5f, 0);
            quitButtonRect.anchorMax = new Vector2(0.5f, 0);
            quitButtonRect.sizeDelta = new Vector2(200, 50);
            quitButtonRect.anchoredPosition = new Vector2(110, 40);
            Image quitButtonImage = quitButtonObj.AddComponent<Image>();
            quitButtonImage.color = Color.red;
            quitButton = quitButtonObj.AddComponent<Button>();
            quitButton.targetGraphic = quitButtonImage;
            quitButton.interactable = true;

            GameObject quitButtonTextObj = new GameObject("QuitButtonText");
            quitButtonTextObj.transform.SetParent(quitButtonObj.transform, false);
            RectTransform quitButtonTextRect = quitButtonTextObj.AddComponent<RectTransform>();
            quitButtonTextRect.anchorMin = Vector2.zero;
            quitButtonTextRect.anchorMax = Vector2.one;
            quitButtonTextRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI quitButtonText = quitButtonTextObj.AddComponent<TextMeshProUGUI>();
            quitButtonText.text = "Quitter";
            quitButtonText.fontSize = 16;
            quitButtonText.alignment = TextAlignmentOptions.Center;
            quitButtonText.color = Color.white;

            restartButton.onClick.AddListener(OnRestartButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);

            Show();
        }
        
        public void ShowResults(bool success, int aileronsFound, int requiredAilerons, float timeElapsed, float riskLevel)
        {
            gameObject.SetActive(true);
    
            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.DisablePlayerControls(true);
            }
    
            if (modalPanel != null)
            {
                modalPanel.SetActive(true);
            }
            else
            {
                Transform panelTransform = transform.Find("EndModalPanel");
                if (panelTransform != null)
                {
                    modalPanel = panelTransform.gameObject;
                    modalPanel.SetActive(true);
                }
            }
    
            // S'assurer que les boutons sont interactifs
            if (restartButton != null)
            {
                restartButton.interactable = true;
        
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }
    
            if (quitButton != null)
            {
                quitButton.interactable = true;
        
                // Réassigner les listeners
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
    
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            string timeString = $"{minutes:00}:{seconds:00}";
    
            if (titleText != null)
            {
                titleText.text = success ? "MISSION ACCOMPLIE" : "TEMPS ÉCOULÉ";
                titleText.color = success ? Color.green : Color.red;
            }
    
            if (resultsText != null)
            {
                if (success)
                    resultsText.text = $"Vous avez validé les {requiredAilerons} ailerons pour Monaco!";
                else
                    resultsText.text = $"Vous avez trouvé seulement {aileronsFound}/{requiredAilerons} ailerons pour Monaco.";
            }
    
            if (timeText != null)
                timeText.text = $"Temps: {timeString}";
        
            if (riskText != null)
            {
                riskText.text = $"Niveau de risque: {riskLevel:0}%";
        
                if (riskLevel < 30f)
                    riskText.color = Color.green;
                else if (riskLevel < 60f)
                    riskText.color = Color.yellow;
                else
                    riskText.color = Color.red;
            }
        }

        public void Show()
        {
            if (modalPanel != null)
                modalPanel.SetActive(true);
        }

        void OnRestartButtonClicked()
        {
            // Cacher le modal
            if (modalPanel != null)
                modalPanel.SetActive(false);
    
            gameObject.SetActive(false);
    
            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.ResetAndStartScenario();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        void OnQuitButtonClicked()
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}