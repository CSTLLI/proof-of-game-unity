using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FeedbackUIController : MonoBehaviour
{
    [Header("Feedback Panel")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image backgroundImage;
    
    [Header("Temporary Message")]
    [SerializeField] private GameObject temporaryMessagePanel;
    [SerializeField] private TextMeshProUGUI temporaryMessageText;
    
    [Header("Colors")]
    [SerializeField] private Color successColor = new Color(0.2f, 0.7f, 0.2f, 0.9f);
    [SerializeField] private Color failureColor = new Color(0.7f, 0.2f, 0.2f, 0.9f);
    [SerializeField] private Color infoColor = new Color(0.2f, 0.2f, 0.7f, 0.9f);
    
    private Coroutine currentTemporaryMessageCoroutine;
    private bool isTabletMode = false;
    
    // Nouvelle méthode d'initialisation pour la création par script
    public void Initialize(GameObject feedbackPanel, GameObject temporaryMessagePanel, TextMeshProUGUI temporaryMessageText)
    {
        this.feedbackPanel = feedbackPanel;
        this.temporaryMessagePanel = temporaryMessagePanel;
        this.temporaryMessageText = temporaryMessageText;
        
        // Si le feedbackPanel existe, configurer ses composants
        if (feedbackPanel != null)
        {
            // Créer le titre s'il n'existe pas
            Transform titleTransform = feedbackPanel.transform.Find("TitleText");
            if (titleTransform == null)
            {
                GameObject titleObj = new GameObject("TitleText");
                titleObj.transform.SetParent(feedbackPanel.transform, false);
                
                RectTransform titleRect = titleObj.AddComponent<RectTransform>();
                titleRect.anchorMin = new Vector2(0, 1);
                titleRect.anchorMax = new Vector2(1, 1);
                titleRect.pivot = new Vector2(0.5f, 1);
                titleRect.anchoredPosition = new Vector2(0, -20);
                titleRect.sizeDelta = new Vector2(-40, 40);
                
                titleText = titleObj.AddComponent<TextMeshProUGUI>();
                titleText.fontSize = 22;
                titleText.alignment = TextAlignmentOptions.Center;
                titleText.color = Color.white;
                titleText.fontStyle = FontStyles.Bold;
            }
            else
            {
                titleText = titleTransform.GetComponent<TextMeshProUGUI>();
            }
            
            // Créer le message s'il n'existe pas
            Transform messageTransform = feedbackPanel.transform.Find("MessageText");
            if (messageTransform == null)
            {
                GameObject messageObj = new GameObject("MessageText");
                messageObj.transform.SetParent(feedbackPanel.transform, false);
                
                RectTransform messageRect = messageObj.AddComponent<RectTransform>();
                messageRect.anchorMin = new Vector2(0, 0);
                messageRect.anchorMax = new Vector2(1, 1);
                messageRect.pivot = new Vector2(0.5f, 0.5f);
                messageRect.anchoredPosition = new Vector2(0, -10);
                messageRect.sizeDelta = new Vector2(-40, -80);
                
                messageText = messageObj.AddComponent<TextMeshProUGUI>();
                messageText.fontSize = 18;
                messageText.alignment = TextAlignmentOptions.Center;
                messageText.color = Color.white;
            }
            else
            {
                messageText = messageTransform.GetComponent<TextMeshProUGUI>();
            }
            
            // Créer le bouton de fermeture s'il n'existe pas
            Transform buttonTransform = feedbackPanel.transform.Find("CloseButton");
            if (buttonTransform == null)
            {
                GameObject buttonObj = new GameObject("CloseButton");
                buttonObj.transform.SetParent(feedbackPanel.transform, false);
                
                RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 0);
                buttonRect.anchorMax = new Vector2(0.5f, 0);
                buttonRect.pivot = new Vector2(0.5f, 0);
                buttonRect.anchoredPosition = new Vector2(0, 30);
                buttonRect.sizeDelta = new Vector2(150, 40);
                
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                closeButton = buttonObj.AddComponent<Button>();
                closeButton.targetGraphic = buttonImage;
                
                // Texte du bouton
                GameObject buttonTextObj = new GameObject("Text");
                buttonTextObj.transform.SetParent(buttonObj.transform, false);
                
                RectTransform textRect = buttonTextObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                
                TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
                buttonText.text = "Fermer";
                buttonText.fontSize = 18;
                buttonText.alignment = TextAlignmentOptions.Center;
                buttonText.color = Color.white;
            }
            else
            {
                closeButton = buttonTransform.GetComponent<Button>();
            }
            
            // Configurer le bouton
            closeButton.onClick.AddListener(CloseFeedbackPanel);
            
            // Récupérer l'image d'arrière-plan
            backgroundImage = feedbackPanel.GetComponent<Image>();
        }
        
        // Détecter si on est sur tablette (ratio d'aspect proche de 4:3)
        float aspectRatio = (float)Screen.width / Screen.height;
        isTabletMode = aspectRatio < 1.5f;
        
        // Ajuster les panels pour la tablette si nécessaire
        if (isTabletMode)
        {
            AdjustForTabletMode();
        }
    }
    
    void Start()
    {
        // Si les références ne sont pas définies, on cherche dans la scène
        if (feedbackPanel == null)
        {
            feedbackPanel = transform.Find("FeedbackPanel")?.gameObject;
        }
        
        if (temporaryMessagePanel == null)
        {
            temporaryMessagePanel = transform.Find("TempMessagePanel")?.gameObject;
        }
        
        // Cacher les panneaux au démarrage
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
            
        if (temporaryMessagePanel != null)
            temporaryMessagePanel.SetActive(false);
            
        // Si les textes ne sont pas définis, on les cherche
        if (titleText == null && feedbackPanel != null)
        {
            titleText = feedbackPanel.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        }
        
        if (messageText == null && feedbackPanel != null)
        {
            messageText = feedbackPanel.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        }
        
        if (temporaryMessageText == null && temporaryMessagePanel != null)
        {
            temporaryMessageText = temporaryMessagePanel.transform.Find("TempMessageText")?.GetComponent<TextMeshProUGUI>();
        }
        
        // Configurer le bouton de fermeture
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseFeedbackPanel);
    }
    
    private void AdjustForTabletMode()
    {
        // Ajuster le panel principal
        if (feedbackPanel != null)
        {
            RectTransform rect = feedbackPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Réduire la taille pour les tablettes
                rect.sizeDelta = new Vector2(rect.sizeDelta.x * 0.8f, rect.sizeDelta.y * 0.8f);
            }
        }
        
        // Ajuster le panel de message temporaire
        if (temporaryMessagePanel != null)
        {
            RectTransform rect = temporaryMessagePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Positionner différemment sur tablette pour éviter les superpositions
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchoredPosition = new Vector2(0, 60);
                rect.sizeDelta = new Vector2(-40, 60);
            }
            
            if (temporaryMessageText != null)
            {
                temporaryMessageText.fontSize = 14; // Taille de police réduite
            }
        }
    }
    
    // Afficher un feedback complet avec titre et message
    public void ShowFeedback(string title, string message, bool isSuccess)
    {
        if (feedbackPanel != null && titleText != null && messageText != null)
        {
            titleText.text = title;
            messageText.text = message;
            
            // Changer la couleur du fond en fonction du résultat
            if (backgroundImage != null)
            {
                backgroundImage.color = isSuccess ? successColor : failureColor;
            }
            
            // Afficher le panneau
            feedbackPanel.SetActive(true);
            
            // Mettre le jeu en pause si nécessaire
            Time.timeScale = 0f; // Pause
        }
    }
    
    // Afficher un message temporaire qui disparaît après un délai
    public void ShowTemporaryMessage(string message, float duration)
    {
        if (temporaryMessagePanel != null && temporaryMessageText != null)
        {
            temporaryMessageText.text = message;
            temporaryMessagePanel.SetActive(true);
            
            // Arrêter les coroutines précédentes pour éviter les conflits
            if (currentTemporaryMessageCoroutine != null)
            {
                StopCoroutine(currentTemporaryMessageCoroutine);
            }
            
            // Démarrer une nouvelle coroutine pour cacher le message après le délai
            currentTemporaryMessageCoroutine = StartCoroutine(HideTemporaryMessageAfterDelay(duration));
            
            // Ajouter une animation simple d'apparition
            CanvasGroup canvasGroup = temporaryMessagePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = temporaryMessagePanel.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 0;
            StartCoroutine(FadeIn(canvasGroup, 0.3f));
        }
    }
    
    // Fermer le panneau de feedback
    public void CloseFeedbackPanel()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
            
            // Reprendre le jeu
            Time.timeScale = 1f;
        }
    }
    
    // Coroutine pour cacher le message temporaire
    private IEnumerator HideTemporaryMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (temporaryMessagePanel != null)
        {
            CanvasGroup canvasGroup = temporaryMessagePanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                yield return StartCoroutine(FadeOut(canvasGroup, 0.3f));
            }
            else
            {
                temporaryMessagePanel.SetActive(false);
            }
        }
    }
    
    // Animations de fade
    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        
        canvasGroup.alpha = 1;
    }
    
    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        
        canvasGroup.alpha = 0;
        temporaryMessagePanel.SetActive(false);
    }
}