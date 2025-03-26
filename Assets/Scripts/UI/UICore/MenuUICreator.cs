using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

public class SimpleMenuUICreator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite ferrariLogo;
    [SerializeField] private AudioClip buttonSound;
    
    [Header("Colors")]
    [SerializeField] private Color ferrariRed = new Color(0.831f, 0.035f, 0.035f);
    [SerializeField] private Color darkBackground = new Color(0.1f, 0.1f, 0.12f, 0.95f);
    
    // UI Canvas reference
    private Canvas menuCanvas;
    
    // UI Panels
    private GameObject loginPanel;
    private GameObject mainMenuPanel;
    
    // Audio Source for UI sounds
    private AudioSource audioSource;
    
    // Current logged in user
    private string currentUsername = "";
    private bool isLoggedIn = false;
    
    void Start()
    {
        // Add audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Create the menu UI
        CreateMenuUI();
    }
    
    public void CreateMenuUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MenuCanvas");
        menuCanvas = canvasObj.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if needed
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create Login Panel
        loginPanel = CreateLoginPanel(canvasObj.transform);
        
        // Create Main Menu Panel
        mainMenuPanel = CreateMainMenuPanel(canvasObj.transform);
        
        // Set initial state
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    
    private GameObject CreateLoginPanel(Transform parent)
    {
        // Create panel
        GameObject panel = new GameObject("LoginPanel");
        panel.transform.SetParent(parent, false);
        
        // Set full screen size
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Add background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(panel.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = darkBackground;
        
        // Add top red gradient
        GameObject topGradient = new GameObject("TopGradient");
        topGradient.transform.SetParent(background.transform, false);
        
        RectTransform gradientRect = topGradient.AddComponent<RectTransform>();
        gradientRect.anchorMin = new Vector2(0, 0.7f);
        gradientRect.anchorMax = new Vector2(1, 1);
        gradientRect.sizeDelta = Vector2.zero;
        
        Image gradientImage = topGradient.AddComponent<Image>();
        gradientImage.color = new Color(ferrariRed.r, ferrariRed.g, ferrariRed.b, 0.2f);
        
        // Add Ferrari logo
        if (ferrariLogo != null)
        {
            GameObject logoObj = new GameObject("FerrariLogo");
            logoObj.transform.SetParent(panel.transform, false);
            
            RectTransform logoRect = logoObj.AddComponent<RectTransform>();
            logoRect.anchorMin = new Vector2(0.5f, 0.85f);
            logoRect.anchorMax = new Vector2(0.5f, 0.85f);
            logoRect.sizeDelta = new Vector2(300, 150);
            logoRect.anchoredPosition = Vector2.zero;
            
            Image logoImage = logoObj.AddComponent<Image>();
            logoImage.sprite = ferrariLogo;
            logoImage.preserveAspect = true;
        }
        
        // Create login card
        GameObject loginCard = new GameObject("LoginCard");
        loginCard.transform.SetParent(panel.transform, false);
        
        RectTransform cardRect = loginCard.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(400, 350);
        cardRect.anchoredPosition = Vector2.zero;
        
        Image cardImage = loginCard.AddComponent<Image>();
        cardImage.color = new Color(0.12f, 0.12f, 0.15f, 0.95f);
        
        // Add title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(loginCard.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 60);
        titleRect.anchoredPosition = new Vector2(0, -30);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "CONNEXION";
        titleText.fontSize = 28;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;
        
        // Add username input
        GameObject usernameInput = CreateInputField(loginCard.transform, "Email / Nom d'utilisateur", 
            new Vector2(0.5f, 0.7f), new Vector2(320, 60));
        
        // Add password input
        GameObject passwordInput = CreateInputField(loginCard.transform, "Mot de passe", 
            new Vector2(0.5f, 0.5f), new Vector2(320, 60));
        
        // Set password input type
        passwordInput.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Password;
        
        // Add login button
        GameObject loginButton = CreateButton(loginCard.transform, "SE CONNECTER", 
            new Vector2(0.5f, 0.3f), new Vector2(280, 60), ferrariRed);
        
        // Add login button listener
        loginButton.GetComponent<Button>().onClick.AddListener(() => {
            PlayButtonSound();
            
            // Get username
            string username = usernameInput.GetComponent<TMP_InputField>().text;
            
            if (!string.IsNullOrEmpty(username))
            {
                currentUsername = username;
                isLoggedIn = true;
                
                // Switch to main menu
                loginPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                
                // Update welcome message
                TextMeshProUGUI welcomeText = mainMenuPanel.transform.Find("TopBar/WelcomeText").GetComponent<TextMeshProUGUI>();
                welcomeText.text = $"Benvenuto, {currentUsername}!";
            }
        });
        
        // Add version text
        GameObject versionObj = new GameObject("VersionText");
        versionObj.transform.SetParent(panel.transform, false);
        
        RectTransform versionRect = versionObj.AddComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(1, 0);
        versionRect.anchorMax = new Vector2(1, 0);
        versionRect.sizeDelta = new Vector2(200, 30);
        versionRect.anchoredPosition = new Vector2(-20, 20);
        
        TextMeshProUGUI versionText = versionObj.AddComponent<TextMeshProUGUI>();
        versionText.text = "Ferrari F1 v1.0.0";
        versionText.fontSize = 14;
        versionText.alignment = TextAlignmentOptions.Right;
        versionText.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        
        return panel;
    }
    
    private GameObject CreateMainMenuPanel(Transform parent)
    {
        // Create panel
        GameObject panel = new GameObject("MainMenuPanel");
        panel.transform.SetParent(parent, false);
        
        // Set full screen size
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Add background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(panel.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = darkBackground;
        
        // Add top gradient
        GameObject topGradient = new GameObject("TopGradient");
        topGradient.transform.SetParent(background.transform, false);
        
        RectTransform gradientRect = topGradient.AddComponent<RectTransform>();
        gradientRect.anchorMin = new Vector2(0, 0.7f);
        gradientRect.anchorMax = new Vector2(1, 1);
        gradientRect.sizeDelta = Vector2.zero;
        
        Image gradientImage = topGradient.AddComponent<Image>();
        gradientImage.color = new Color(ferrariRed.r, ferrariRed.g, ferrariRed.b, 0.2f);
        
        // Create top bar
        GameObject topBar = new GameObject("TopBar");
        topBar.transform.SetParent(panel.transform, false);
        
        RectTransform topBarRect = topBar.AddComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0, 1);
        topBarRect.anchorMax = new Vector2(1, 1);
        topBarRect.sizeDelta = new Vector2(0, 80);
        topBarRect.anchoredPosition = Vector2.zero;
        
        Image topBarImage = topBar.AddComponent<Image>();
        topBarImage.color = new Color(0.1f, 0.1f, 0.12f, 0.8f);
        
        // Add welcome text
        GameObject welcomeObj = new GameObject("WelcomeText");
        welcomeObj.transform.SetParent(topBar.transform, false);
        
        RectTransform welcomeRect = welcomeObj.AddComponent<RectTransform>();
        welcomeRect.anchorMin = new Vector2(0, 0);
        welcomeRect.anchorMax = new Vector2(1, 1);
        welcomeRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI welcomeText = welcomeObj.AddComponent<TextMeshProUGUI>();
        welcomeText.text = "Benvenuto!";
        welcomeText.fontSize = 24;
        welcomeText.alignment = TextAlignmentOptions.Center;
        welcomeText.color = Color.white;
        
        // Create menu container
        GameObject menuContainer = new GameObject("MenuContainer");
        menuContainer.transform.SetParent(panel.transform, false);
        
        RectTransform menuRect = menuContainer.AddComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.5f, 0.5f);
        menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.sizeDelta = new Vector2(500, 500);
        menuRect.anchoredPosition = Vector2.zero;
        
        // Create title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(menuContainer.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 60);
        titleRect.anchoredPosition = new Vector2(0, 0);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "FERRARI F1 - CONTRÔLE QUALITÉ";
        titleText.fontSize = 28;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Create play button
        GameObject playButton = CreateButton(menuContainer.transform, "JOUER", 
            new Vector2(0.5f, 0.7f), new Vector2(320, 70), ferrariRed);
        
        // Add play button listener
        playButton.GetComponent<Button>().onClick.AddListener(() => {
            PlayButtonSound();
            
            // Start the game
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.StartGame();
            }
        });
        
        // Create store button
        GameObject storeButton = CreateButton(menuContainer.transform, "BOUTIQUE", 
            new Vector2(0.5f, 0.5f), new Vector2(320, 70), new Color(0.2f, 0.2f, 0.25f, 1));
        
        // Create settings button
        GameObject settingsButton = CreateButton(menuContainer.transform, "PARAMÈTRES", 
            new Vector2(0.5f, 0.3f), new Vector2(320, 70), new Color(0.2f, 0.2f, 0.25f, 1));
        
        // Create bottom bar
        GameObject bottomBar = new GameObject("BottomBar");
        bottomBar.transform.SetParent(panel.transform, false);
        
        RectTransform bottomBarRect = bottomBar.AddComponent<RectTransform>();
        bottomBarRect.anchorMin = new Vector2(0, 0);
        bottomBarRect.anchorMax = new Vector2(1, 0);
        bottomBarRect.sizeDelta = new Vector2(0, 60);
        bottomBarRect.anchoredPosition = Vector2.zero;
        
        Image bottomBarImage = bottomBar.AddComponent<Image>();
        bottomBarImage.color = new Color(0.1f, 0.1f, 0.12f, 0.8f);
        
// Create logout button
        GameObject logoutButton = CreateButton(bottomBar.transform, "DÉCONNEXION", 
            new Vector2(0.1f, 0.5f), new Vector2(200, 40), new Color(0.6f, 0.1f, 0.1f, 1));
        
        // Add logout button listener
        logoutButton.GetComponent<Button>().onClick.AddListener(() => {
            PlayButtonSound();
            
            // Logout
            currentUsername = "";
            isLoggedIn = false;
            
            // Switch back to login panel
            mainMenuPanel.SetActive(false);
            loginPanel.SetActive(true);
        });
        
        // Add copyright text
        GameObject copyrightObj = new GameObject("CopyrightText");
        copyrightObj.transform.SetParent(bottomBar.transform, false);
        
        RectTransform copyrightRect = copyrightObj.AddComponent<RectTransform>();
        copyrightRect.anchorMin = new Vector2(1, 0.5f);
        copyrightRect.anchorMax = new Vector2(1, 0.5f);
        copyrightRect.sizeDelta = new Vector2(300, 40);
        copyrightRect.anchoredPosition = new Vector2(-20, 0);
        
        TextMeshProUGUI copyrightText = copyrightObj.AddComponent<TextMeshProUGUI>();
        copyrightText.text = "© 2025 Ferrari S.p.A.";
        copyrightText.fontSize = 14;
        copyrightText.alignment = TextAlignmentOptions.Right;
        copyrightText.color = new Color(0.7f, 0.7f, 0.7f);
        
        return panel;
    }
    
    private GameObject CreateInputField(Transform parent, string placeholder, Vector2 anchorPosition, Vector2 size)
    {
        // Create input field container
        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(parent, false);
        
        RectTransform inputRect = inputObj.AddComponent<RectTransform>();
        inputRect.anchorMin = anchorPosition;
        inputRect.anchorMax = anchorPosition;
        inputRect.sizeDelta = size;
        inputRect.anchoredPosition = Vector2.zero;
        
        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = new Color(0.08f, 0.08f, 0.1f);
        
        // Create input field
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        
        // Create text area
        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(inputObj.transform, false);
        
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = new Vector2(0, 0);
        textAreaRect.anchorMax = new Vector2(1, 1);
        textAreaRect.sizeDelta = new Vector2(-20, -10);
        textAreaRect.anchoredPosition = Vector2.zero;
        
        // Create text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(textArea.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = "";
        textComponent.fontSize = 18;
        textComponent.color = Color.white;
        
        // Create placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(textArea.transform, false);
        
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.anchoredPosition = new Vector2(10, 0);
        
        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 18;
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0.6f, 0.6f, 0.6f);
        
        // Set up input field component
        inputField.textComponent = textComponent;
        inputField.placeholder = placeholderText;
        inputField.textViewport = textAreaRect;
        
        return inputObj;
    }
    
    private GameObject CreateButton(Transform parent, string label, Vector2 anchorPosition, Vector2 size, Color color)
    {
        // Create button object
        GameObject buttonObj = new GameObject("Button_" + label);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorPosition;
        buttonRect.anchorMax = anchorPosition;
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = Vector2.zero;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;
        
        // Add button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Set up button colors
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(
            Mathf.Min(color.r + 0.1f, 1f),
            Mathf.Min(color.g + 0.1f, 1f),
            Mathf.Min(color.b + 0.1f, 1f)
        );
        colors.pressedColor = new Color(
            Mathf.Max(color.r - 0.1f, 0f),
            Mathf.Max(color.g - 0.1f, 0f),
            Mathf.Max(color.b - 0.1f, 0f)
        );
        button.colors = colors;
        
        // Create text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = label;
        buttonText.fontSize = 20;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        return buttonObj;
    }
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}