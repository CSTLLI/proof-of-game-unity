using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUICreator : MonoBehaviour
{
    [SerializeField] private Font defaultFont;
    
    // Variables pour configurer l'UI
    [SerializeField] private string scenarioName = "Mission par défaut";
    [SerializeField] private float totalGameTime = 300f; // 5 minutes en secondes
    
    // Références pour garder trace des éléments créés
    private Canvas gameCanvas;
    private TextMeshProUGUI scenarioText;
    private TextMeshProUGUI timerText;
    private Slider progressBar;
    private GameUIManager uiManager;
    
    void Awake()
    {
        CreateGameUI();
    }
    
    private void CreateGameUI()
    {
        // 1. Créer le Canvas principal
        GameObject canvasObj = new GameObject("GameUI_Canvas");
        gameCanvas = canvasObj.AddComponent<Canvas>();
        gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Ajouter les composants nécessaires au Canvas
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 2. Ajouter un EventSystem s'il n'en existe pas déjà un
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // 3. Créer le panneau de nom du scénario (en haut)
        GameObject scenarioPanel = CreatePanel(canvasObj, "ScenarioPanel", new Vector2(0.5f, 1), new Vector2(0.5f, 1), 
            new Vector2(0, -50), new Vector2(400, 50));
        
        scenarioText = CreateTextElement(scenarioPanel, scenarioName, 24, TextAlignmentOptions.Center);
        scenarioText.rectTransform.anchorMin = new Vector2(0, 0);
        scenarioText.rectTransform.anchorMax = new Vector2(1, 1);
        scenarioText.rectTransform.offsetMin = new Vector2(10, 10);
        scenarioText.rectTransform.offsetMax = new Vector2(-10, -10);
        
        // 4. Créer le panneau de timer (en haut à droite)
        GameObject timerPanel = CreatePanel(canvasObj, "TimerPanel", new Vector2(1, 1), new Vector2(1, 1), 
                                           new Vector2(-100, -50), new Vector2(140, 50));
        
        timerText = CreateTextElement(timerPanel, "05:00", 28, TextAlignmentOptions.Center);
        timerText.rectTransform.anchorMin = new Vector2(0, 0);
        timerText.rectTransform.anchorMax = new Vector2(1, 1);
        timerText.rectTransform.offsetMin = new Vector2(5, 5);
        timerText.rectTransform.offsetMax = new Vector2(-5, -5);
        
        // 5. Créer la barre de progression (en bas)
        GameObject progressPanel = CreatePanel(canvasObj, "ProgressPanel", new Vector2(0.5f, 0), new Vector2(0.5f, 0), 
                                              new Vector2(0, 50), new Vector2(400, 40));
        progressPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
        
        // Créer la barre de progression
        progressBar = CreateProgressBar(progressPanel);
        
        // 6. Ajouter le GameUIManager pour gérer l'UI
        uiManager = canvasObj.AddComponent<GameUIManager>();
        uiManager.Initialize(scenarioText, timerText, progressBar, scenarioName, totalGameTime);
        
        // Si on veut que l'UI persiste entre les scènes
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
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.7f);
        
        return panel;
    }
    
    private TextMeshProUGUI CreateTextElement(GameObject parent, string text, int fontSize, 
                                             TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return textComponent;
    }
    
    private Slider CreateProgressBar(GameObject parent)
    {
        // Panel principal pour la barre de progression
        GameObject sliderObj = new GameObject("ProgressBar");
        sliderObj.transform.SetParent(parent.transform, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.wholeNumbers = false;
        
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 0);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.offsetMin = new Vector2(10, 10);
        sliderRect.offsetMax = new Vector2(-10, -10);
        
        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 1);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0);
        fillAreaRect.anchorMax = new Vector2(1, 1);
        fillAreaRect.offsetMin = new Vector2(5, 5);
        fillAreaRect.offsetMax = new Vector2(-5, -5);
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.7f, 0.2f, 1f);
        
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        slider.fillRect = fillRect;
        slider.targetGraphic = bgImage;
        
        return slider;
    }
}