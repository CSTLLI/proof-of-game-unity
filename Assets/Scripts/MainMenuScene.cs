using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarioManager;
    [SerializeField] private Sprite ferrariLogo;
    [SerializeField] private bool requireLogin = true;
    
    private GameObject menuCanvas;
    private LoginUIManager loginManager;
    
    private void Start()
    {
        // Désactiver les contrôles du joueur en attendant le démarrage du jeu
        if (scenarioManager == null)
        {
            scenarioManager = FindObjectOfType<ScenarioManager>();
        }
        
        if (scenarioManager != null)
        {
            scenarioManager.DisablePlayerControls(true);
        }
        
        // Créer l'UI du menu
        CreateMenuUI();
        
        if (requireLogin)
        {
            InitializeLoginManager();
        }
    }
    
    private void CreateMenuUI()
    {
        // Créer un canvas
        menuCanvas = new GameObject("MenuCanvas");
        Canvas canvas = menuCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        menuCanvas.AddComponent<GraphicRaycaster>();
        
        // Ajouter un fond
        GameObject background = new GameObject("Background");
        background.transform.SetParent(menuCanvas.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.12f, 0.95f);
        
        // Ajouter le logo Ferrari si disponible
        if (ferrariLogo != null)
        {
            GameObject logoObj = new GameObject("FerrariLogo");
            logoObj.transform.SetParent(menuCanvas.transform, false);
            RectTransform logoRect = logoObj.AddComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0.5f, 0.7f);
            logoRect.anchorMax = new Vector2(0.5f, 0.7f);
            logoRect.sizeDelta = new Vector2(300, 150);
            logoRect.anchoredPosition = Vector2.zero;
            Image logoImage = logoObj.AddComponent<Image>();
            logoImage.sprite = ferrariLogo;
            logoImage.preserveAspect = true;
        }
        
        // Ajouter un titre
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(menuCanvas.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 50);
        titleRect.sizeDelta = new Vector2(600, 100);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PROOF OF GAME";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.95f, 0.95f, 0.95f);
        titleText.fontStyle = FontStyles.Bold;
        
        // Sous-titre
        GameObject subtitleObj = new GameObject("Subtitle");
        subtitleObj.transform.SetParent(menuCanvas.transform, false);
        RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.anchoredPosition = new Vector2(0, 0);
        subtitleRect.sizeDelta = new Vector2(600, 50);
        TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
        subtitleText.text = "Plateforme de simulation professionnelle";
        subtitleText.fontSize = 20;
        subtitleText.alignment = TextAlignmentOptions.Center;
        subtitleText.color = new Color(0.8f, 0.8f, 0.8f);
        
        if (!requireLogin)
        {
            CreatePlayButton();
        }
        else
        {
            CreateLoginButton();
        }
    }
    
    private void CreatePlayButton()
    {
        GameObject buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(menuCanvas.transform, false);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, -50);
        buttonRect.sizeDelta = new Vector2(250, 60);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.831f, 0.035f, 0.035f); // Rouge Ferrari
        
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.anchoredPosition = Vector2.zero;
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "JOUER - SCENARIO FERRARI F1";
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;
        
        Button playButton = buttonObj.AddComponent<Button>();
        playButton.onClick.AddListener(StartGame);
        
        ColorBlock colors = playButton.colors;
        colors.highlightedColor = new Color(0.93f, 0.18f, 0.18f);
        colors.pressedColor = new Color(0.7f, 0.03f, 0.03f);
        playButton.colors = colors;
    }
    
    private void CreateLoginButton()
    {
        GameObject buttonObj = new GameObject("LoginButton");
        buttonObj.transform.SetParent(menuCanvas.transform, false);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, -50);
        buttonRect.sizeDelta = new Vector2(250, 60);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.831f, 0.035f, 0.035f); // Rouge Ferrari
        
        // Ajouter le texte du bouton
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.anchoredPosition = Vector2.zero;
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "CONNEXION";
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;
        
        Button loginButton = buttonObj.AddComponent<Button>();
        loginButton.onClick.AddListener(ShowLogin);
        
        ColorBlock colors = loginButton.colors;
        colors.highlightedColor = new Color(0.93f, 0.18f, 0.18f);
        colors.pressedColor = new Color(0.7f, 0.03f, 0.03f);
        loginButton.colors = colors;
    }
    
    private void InitializeLoginManager()
    {
        GameObject loginManagerObj = new GameObject("LoginUIManager");
        loginManager = loginManagerObj.AddComponent<LoginUIManager>();
        
        loginManager.CreateUIElements();
        loginManager.HideAllPanels();
        
        loginManager.onLogout += ShowMainMenu;
        
        loginManager.gameObject.SetActive(false);
    }
    
    private void ShowLogin()
    {
        if (loginManager != null)
        {
            menuCanvas.SetActive(false);
            
            loginManager.gameObject.SetActive(true);
            loginManager.ShowLoginPanel();
            
            loginManager.onSuccessfulLogin = () => {
                loginManager.ShowMainMenuPanel();
            };
        }
    }
    
    private void ShowMainMenu()
    {
        if (loginManager != null)
        {
            loginManager.gameObject.SetActive(false);
        }
        
        // Afficher le menu principal
        menuCanvas.SetActive(true);
    }
    
    private void StartGame()
    {
        // Cacher tous les éléments d'UI
        menuCanvas.SetActive(false);
        if (loginManager != null)
        {
            loginManager.gameObject.SetActive(false);
        }
        
        if (scenarioManager != null)
        {
            scenarioManager.StartScenario();
        }
        else
        {
            Debug.LogError("ScenarioManager non trouvé!");
        }
    }
}