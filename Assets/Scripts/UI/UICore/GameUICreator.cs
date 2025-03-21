using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.UICore
{
    public class GameUICreator : MonoBehaviour
    {
        [SerializeField] private float totalGameTime = 300f;
        
        private Canvas gameCanvas;
        private TextMeshProUGUI timerText;
        private Slider progressBar;
        private TextMeshProUGUI taskText;
        private GameUIManager uiManager;
        private GameObject canvasObj;
        
        void Awake()
        {
            // Créer l'UI mais ne pas l'afficher immédiatement
            CreateModernUI();
            SetUIActive(false); // Désactiver l'UI par défaut
            
            ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
            if (scenarioManager != null)
            {
                // Utiliser un Reflection pour assigner le champ privé/sérialisé
                System.Reflection.FieldInfo field = typeof(ScenarioManager).GetField("gameUIManager", 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Public);
                if (field != null)
                {
                    field.SetValue(scenarioManager, uiManager);
                }
            }
        }
        
        public void SetUIActive(bool active)
        {
            if (canvasObj != null)
            {
                canvasObj.SetActive(active);
            }
        }

        public void StartGame()
        {
            SetUIActive(true);
        }
        
        private void CreateModernUI()
        {
            GameObject canvasObj = new GameObject("GameUI_Canvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();
            
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            GameObject taskPanel = CreatePanel(canvasObj, "TaskPanel", new Vector2(0, 1), new Vector2(0, 1),
                                             new Vector2(20, -20), new Vector2(350, 120));
            
            GameObject logoObj = new GameObject("Logo");
            logoObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform logoRect = logoObj.AddComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0, 1);
            logoRect.anchorMax = new Vector2(0, 1);
            logoRect.pivot = new Vector2(0, 1);
            logoRect.anchoredPosition = new Vector2(10, -10);
            logoRect.sizeDelta = new Vector2(30, 30);
            
            Image logoImage = logoObj.AddComponent<Image>();
            logoImage.color = new Color(0.8f, 0.1f, 0.1f, 1f);
            
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(0.7f, 1);
            titleRect.pivot = new Vector2(0, 1);
            titleRect.anchoredPosition = new Vector2(50, -15);
            titleRect.sizeDelta = new Vector2(0, 30);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Contrôle Qualité F1";
            titleText.fontSize = 18;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Left;
            
            GameObject timerObj = new GameObject("TimerText");
            timerObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform timerRect = timerObj.AddComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.7f, 1);
            timerRect.anchorMax = new Vector2(1, 1);
            timerRect.pivot = new Vector2(1, 1);
            timerRect.anchoredPosition = new Vector2(-10, -15);
            timerRect.sizeDelta = new Vector2(0, 30);
            
            timerText = timerObj.AddComponent<TextMeshProUGUI>();
            timerText.text = "05:00";
            timerText.fontSize = 20;
            timerText.color = Color.white;
            timerText.alignment = TextAlignmentOptions.Right;
            
            GameObject descObj = new GameObject("TaskText");
            descObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.3f);
            descRect.anchorMax = new Vector2(1, 0.8f);
            descRect.pivot = new Vector2(0.5f, 0.5f);
            descRect.sizeDelta = Vector2.zero;
            
            taskText = descObj.AddComponent<TextMeshProUGUI>();
            taskText.text = "Trouvez et validez les 2 ailerons Monaco parmi les 6 disponibles";
            taskText.fontSize = 16;
            taskText.color = Color.white;
            taskText.alignment = TextAlignmentOptions.Left;
            
            GameObject blockchainToggleObj = new GameObject("BlockchainToggle");
            blockchainToggleObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform toggleRect = blockchainToggleObj.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0, 0);
            toggleRect.anchorMax = new Vector2(0.5f, 0.3f);
            toggleRect.pivot = new Vector2(0, 0);
            toggleRect.anchoredPosition = new Vector2(10, 10);
            toggleRect.sizeDelta = new Vector2(0, 30);
            
            Toggle blockchainToggle = blockchainToggleObj.AddComponent<Toggle>();
            
            GameObject toggleBg = new GameObject("Background");
            toggleBg.transform.SetParent(blockchainToggleObj.transform, false);
            
            RectTransform bgRect = toggleBg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.5f);
            bgRect.anchorMax = new Vector2(0, 0.5f);
            bgRect.pivot = new Vector2(0, 0.5f);
            bgRect.anchoredPosition = new Vector2(10, 0);
            bgRect.sizeDelta = new Vector2(20, 20);
            
            Image bgImage = toggleBg.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            GameObject toggleCheck = new GameObject("Checkmark");
            toggleCheck.transform.SetParent(toggleBg.transform, false);
            
            RectTransform checkRect = toggleCheck.AddComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.sizeDelta = new Vector2(-4, -4);
            
            Image checkImage = toggleCheck.AddComponent<Image>();
            checkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            GameObject toggleLabel = new GameObject("Label");
            toggleLabel.transform.SetParent(blockchainToggleObj.transform, false);
            
            RectTransform labelRect = toggleLabel.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.pivot = new Vector2(0, 0.5f);
            labelRect.anchoredPosition = new Vector2(40, 0);
            
            TextMeshProUGUI labelText = toggleLabel.AddComponent<TextMeshProUGUI>();
            labelText.text = "Mode Blockchain";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            
            blockchainToggle.graphic = checkImage;
            blockchainToggle.targetGraphic = bgImage;
            
            GameObject progressObj = new GameObject("ProgressPanel");
            progressObj.transform.SetParent(taskPanel.transform, false);
            
            RectTransform progressRect = progressObj.AddComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0.5f, 0);
            progressRect.anchorMax = new Vector2(1, 0.3f);
            progressRect.pivot = new Vector2(0.5f, 0);
            progressRect.anchoredPosition = new Vector2(60, 10);
            progressRect.sizeDelta = Vector2.zero;
            
            progressBar = CreateProgressBar(progressObj);
            
            uiManager = canvasObj.AddComponent<GameUIManager>();
            uiManager.Initialize(titleText, timerText, progressBar, "Contrôle Qualité F1", totalGameTime);
            
            DontDestroyOnLoad(canvasObj);
        }
        
        private GameObject CreatePanel(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax,
                                     Vector2 position, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent.transform, false);
            
            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;
            rectTransform.pivot = new Vector2(0, 1);
            
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.7f);
            
            return panel;
        }
        
        private Slider CreateProgressBar(GameObject parent)
        {
            GameObject sliderObj = new GameObject("ProgressBar");
            sliderObj.transform.SetParent(parent.transform, false);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
            
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.sizeDelta = Vector2.zero;
            
            GameObject background = new GameObject("Background");
            background.transform.SetParent(sliderObj.transform, false);
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(sliderObj.transform, false);
            
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.8f, 0.1f, 0.1f, 1f);
            
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.sizeDelta = new Vector2(0, 0);
            
            slider.fillRect = fillRect;
            
            return slider;
        }
    }
}