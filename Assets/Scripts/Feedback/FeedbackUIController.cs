using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FeedbackUIController : MonoBehaviour
{
    [Header("UI References")]
    private GameObject feedbackPanel;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI messageText;
    private Button closeButton;
    private Button restartButton;
    
    [Header("Appearance")]
    [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color failureColor = new Color(0.8f, 0.2f, 0.2f);
    
    private bool isInitialized = false;
    
    public void ShowFeedback(string title, string message, bool isSuccess)
    {
        if (!isInitialized)
        {
            CreateFeedbackUI();
        }
        
        // Configurer le contenu
        titleText.text = title;
        titleText.color = isSuccess ? successColor : failureColor;
        messageText.text = message;
        
        // Afficher le panel
        feedbackPanel.SetActive(true);
        
        // Jouer un son si nécessaire
        // AudioSource audio = GetComponent<AudioSource>();
        // if (audio != null)
        // {
        //     audio.PlayOneShot(isSuccess ? successSound : failureSound);
        // }
    }
    
    private void CreateFeedbackUI()
    {
        // Vérifier si l'UI existe déjà
        GameObject existingCanvas = GameObject.Find("FeedbackCanvas");
        if (existingCanvas != null)
        {
            Debug.LogWarning("FeedbackCanvas existe déjà, utilisation de l'existant.");
            feedbackPanel = existingCanvas.transform.Find("FeedbackPanel").gameObject;
            titleText = feedbackPanel.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            messageText = feedbackPanel.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
            closeButton = feedbackPanel.transform.Find("CloseButton").GetComponent<Button>();
            restartButton = feedbackPanel.transform.Find("RestartButton").GetComponent<Button>();
            
            isInitialized = true;
            return;
        }
        
        // Créer le Canvas
        GameObject canvasObject = new GameObject("FeedbackCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 110; // Au-dessus des autres UI
        
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Créer un fond semi-transparent pour bloquer les interactions
        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvasObject.transform, false);
        
        RectTransform overlayRect = overlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.7f);
        
        // Créer le panel
        feedbackPanel = new GameObject("FeedbackPanel");
        feedbackPanel.transform.SetParent(canvasObject.transform, false);
        
        RectTransform panelRect = feedbackPanel.AddComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(600, 400);
        
        Image panelImage = feedbackPanel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        
        // Titre
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(feedbackPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.anchoredPosition = new Vector2(0, -40);
        titleRect.sizeDelta = new Vector2(0, 60);
        
        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 28;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        // Message
        GameObject messageObj = new GameObject("MessageText");
        messageObj.transform.SetParent(feedbackPanel.transform, false);
        
        RectTransform messageRect = messageObj.AddComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0, 0);
        messageRect.anchorMax = new Vector2(1, 1);
        messageRect.offsetMin = new Vector2(40, 100);
        messageRect.offsetMax = new Vector2(-40, -80);
        
        messageText = messageObj.AddComponent<TextMeshProUGUI>();
        messageText.fontSize = 18;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.white;
        
        // Bouton fermer
        closeButton = CreateButton("CloseButton", feedbackPanel.transform, 
            new Vector2(80, -150), new Vector2(160, 50), "Fermer");
        closeButton.onClick.AddListener(CloseFeedback);
        
        // Bouton redémarrer
        restartButton = CreateButton("RestartButton", feedbackPanel.transform, 
            new Vector2(-80, -150), new Vector2(160, 50), "Recommencer");
        restartButton.onClick.AddListener(RestartScenario);
        
        isInitialized = true;
    }
    
    private Button CreateButton(string name, Transform parent, Vector2 position, Vector2 size, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.3f);
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f);
        button.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return button;
    }
    
    public void CloseFeedback()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
    
    public void RestartScenario()
    {
        // Trouver le ScenarioManager et réinitialiser le scénario
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            // Si vous avez une méthode de redémarrage dans le ScenarioManager
            // scenarioManager.RestartScenario();
            
            // Sinon, recharger la scène
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        CloseFeedback();
    }
}