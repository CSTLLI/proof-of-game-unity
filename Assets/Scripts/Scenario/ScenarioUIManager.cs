using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScenarioUIManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool createUIOnStart = true;
    
    // Références aux objets UI créés dynamiquement
    private GameObject taskIndicatorCanvas;
    private GameObject feedbackCanvas;
    private CurrentTaskUpdater taskUpdater;
    private FeedbackUIController feedbackController;
    
    // Singleton pour accès facile
    public static ScenarioUIManager Instance { get; private set; }
    
    void Awake()
    {
        // Configuration du singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (createUIOnStart)
        {
            StartCoroutine(SetupAllUI());
        }
    }
    
    private IEnumerator SetupAllUI()
    {
        // Attendre une frame pour s'assurer que tous les managers sont initialisés
        yield return null;
        
        // Créer tous les éléments UI nécessaires
        CreateTaskIndicator();
        CreateFeedbackSystem();
        
        // Attendre encore une frame pour que tout soit initialisé
        yield return null;
        
        // Connecter les composants entre eux
        ConnectComponents();
        
        // Afficher un message de bienvenue
        if (feedbackController != null)
        {
            ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
            string scenarioName = scenarioManager != null ? scenarioManager.GetScenarioName() : "Contrôle Qualité F1";
            
            feedbackController.ShowTemporaryMessage($"Bienvenue dans le scénario: {scenarioName}. Consultez votre liste de tâches pour commencer.", 5f);
        }
    }
    
    private void CreateTaskIndicator()
    {
        // Vérifier si l'indicateur existe déjà
        if (GameObject.Find("TaskIndicatorCanvas") != null)
            return;
            
        // Créer le Canvas
        taskIndicatorCanvas = new GameObject("TaskIndicatorCanvas");
        Canvas canvas = taskIndicatorCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        taskIndicatorCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        taskIndicatorCanvas.AddComponent<GraphicRaycaster>();
        
        // Créer le panel d'indicateur
        GameObject indicatorPanel = new GameObject("CurrentTaskPanel");
        indicatorPanel.transform.SetParent(taskIndicatorCanvas.transform, false);
        
        RectTransform rect = indicatorPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(300, 100);
        
        Image background = indicatorPanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f);
        
        // Ajouter le texte de titre
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(indicatorPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -5);
        titleRect.sizeDelta = new Vector2(-20, 30);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Tâche actuelle:";
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Left;
        
        // Ajouter le texte de description
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(indicatorPanel.transform, false);
        
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.anchoredPosition = new Vector2(0, -15);
        descRect.sizeDelta = new Vector2(-20, -40);
        
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Consultez votre liste de tâches...";
        descText.fontSize = 14;
        descText.color = Color.white;
        descText.alignment = TextAlignmentOptions.Left;
        
        // Ajouter le script de mise à jour de la tâche
        taskUpdater = indicatorPanel.AddComponent<CurrentTaskUpdater>();
        
        // Ne pas détruire entre les scènes
        DontDestroyOnLoad(taskIndicatorCanvas);
    }
    
    private void CreateFeedbackSystem()
    {
        // Vérifier si le système de feedback existe déjà
        if (GameObject.Find("FeedbackCanvas") != null)
            return;
            
        // Créer le Canvas
        feedbackCanvas = new GameObject("FeedbackCanvas");
        Canvas canvas = feedbackCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // S'assurer qu'il est au-dessus de tout
        
        feedbackCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        feedbackCanvas.AddComponent<GraphicRaycaster>();
        
        // Créer le panel de feedback
        GameObject feedbackPanel = new GameObject("FeedbackPanel");
        feedbackPanel.transform.SetParent(feedbackCanvas.transform, false);
        
        RectTransform panelRect = feedbackPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(400, 250);
        
        Image panelImage = feedbackPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Créer le panel de message temporaire
        GameObject tempMsgPanel = new GameObject("TempMessagePanel");
        tempMsgPanel.transform.SetParent(feedbackCanvas.transform, false);
        
        RectTransform tempRect = tempMsgPanel.AddComponent<RectTransform>();
        tempRect.anchorMin = new Vector2(0.5f, 0);
        tempRect.anchorMax = new Vector2(0.5f, 0);
        tempRect.pivot = new Vector2(0.5f, 0);
        tempRect.anchoredPosition = new Vector2(0, 80);
        tempRect.sizeDelta = new Vector2(500, 60);
        
        Image tempImage = tempMsgPanel.AddComponent<Image>();
        tempImage.color = new Color(0, 0, 0, 0.7f);
        
        // Ajouter le texte temporaire
        GameObject tempTextObj = new GameObject("TempMessageText");
        tempTextObj.transform.SetParent(tempMsgPanel.transform, false);
        
        RectTransform tempTextRect = tempTextObj.AddComponent<RectTransform>();
        tempTextRect.anchorMin = Vector2.zero;
        tempTextRect.anchorMax = Vector2.one;
        tempTextRect.offsetMin = new Vector2(10, 5);
        tempTextRect.offsetMax = new Vector2(-10, -5);
        
        TextMeshProUGUI tempText = tempTextObj.AddComponent<TextMeshProUGUI>();
        tempText.alignment = TextAlignmentOptions.Center;
        tempText.fontSize = 16;
        tempText.color = Color.white;
        
        // Ajouter le contrôleur
        feedbackController = feedbackCanvas.AddComponent<FeedbackUIController>();
        
        // Configurer le contrôleur
        feedbackController.Initialize(feedbackPanel, tempMsgPanel, tempText);
        
        // Désactiver les panels au départ
        feedbackPanel.SetActive(false);
        tempMsgPanel.SetActive(false);
        
        // Ne pas détruire entre les scènes
        DontDestroyOnLoad(feedbackCanvas);
    }
    
    private void ConnectComponents()
    {
        // Récupérer le ScenarioManager
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null && taskUpdater != null)
        {
            // Le taskUpdater est déjà créé, on n'a pas besoin de le recréer.
            // Initialiser le taskUpdater avec le ScenarioManager
            // On doit trouver les composants TextMeshProUGUI manuellement
            if (taskIndicatorCanvas != null)
            {
                Transform panelTransform = taskIndicatorCanvas.transform.Find("CurrentTaskPanel");
                if (panelTransform != null)
                {
                    TextMeshProUGUI titleText = panelTransform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI descText = panelTransform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
                    
                    if (titleText != null && descText != null)
                    {
                        taskUpdater.Initialize(titleText, descText, scenarioManager);
                    }
                    else
                    {
                        Debug.LogError("Impossible de trouver les composants TextMeshProUGUI nécessaires");
                    }
                }
            }
        }
    }
    
    // Méthodes utilitaires pouvant être appelées de n'importe où
    public void ShowTemporaryMessage(string message, float duration = 3f)
    {
        if (feedbackController != null)
        {
            feedbackController.ShowTemporaryMessage(message, duration);
        }
    }
    
    public CurrentTaskUpdater GetTaskUpdater()
    {
        return taskUpdater;
    }
    
    public FeedbackUIController GetFeedbackController()
    {
        return feedbackController;
    }
}