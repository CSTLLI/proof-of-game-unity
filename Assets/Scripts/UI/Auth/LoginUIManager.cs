using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class LoginUIManager : MonoBehaviour
{
    [Header("Panels")] public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;

    [Header("Login Panel")] public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;
    public Button toRegisterButton;
    public TextMeshProUGUI loginErrorText;

    [Header("Register Panel")] public TMP_InputField registerNameInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerConfirmPasswordInput;
    public Button registerButton;
    public Button toLoginButton;
    public TextMeshProUGUI registerErrorText;

    [Header("Main Menu Panel")] public TextMeshProUGUI welcomeText;
    public Button playButton;
    public Button logoutButton;

    [Header("Audio")] [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    private AudioSource audioSource;
    public string currentUserName = "";

    public Action onSuccessfulLogin;
    public event Action onLogout;

    private LoginManager loginManager;

    public bool IsLoggedIn { get; set; } = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (loginPanel == null)
        {
            CreateUIElements();
        }

        Debug.Log("Start: LoginUI");
        InitializeUI();
    }

    public void CreateUIElements()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();
        }

        loginPanel = CreateLoginPanel();
        registerPanel = CreateRegisterPanel();
        mainMenuPanel = CreateMainMenuPanel();
        loadingPanel = CreateLoadingPanel();
    }

    private GameObject CreateLoginPanel()
    {
        // Panel de connexion
        GameObject panel = CreateBasePanel("LoginPanel");

        // Titre
        TextMeshProUGUI titleText = CreateText(panel, "TitleText", "CONNEXION", 24,
            new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f),
            new Vector2(0, 0), new Vector2(300, 50));

        // Email
        TextMeshProUGUI usernameLabel = CreateText(panel, "UsernameLabel", "Pseudo :", 16,
            new Vector2(0.3f, 0.7f), new Vector2(0.3f, 0.7f),
            new Vector2(0, 0), new Vector2(150, 30));

        loginUsernameInput = CreateInputField(panel, "UsernameInput", "Entrez votre pseudo",
            new Vector2(0.7f, 0.7f), new Vector2(0.7f, 0.7f),
            new Vector2(0, 0), new Vector2(200, 40));

        // Mot de passe
        TextMeshProUGUI passwordLabel = CreateText(panel, "PasswordLabel", "Mot de passe :", 16,
            new Vector2(0.3f, 0.6f), new Vector2(0.3f, 0.6f),
            new Vector2(0, 0), new Vector2(150, 30));

        loginPasswordInput = CreateInputField(panel, "PasswordInput", "Entrez votre mot de passe",
            new Vector2(0.7f, 0.6f), new Vector2(0.7f, 0.6f),
            new Vector2(0, 0), new Vector2(200, 40));
        loginPasswordInput.contentType = TMP_InputField.ContentType.Password;

        // Message d'erreur
        loginErrorText = CreateText(panel, "ErrorText", "", 14,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 0), new Vector2(400, 30));
        loginErrorText.color = Color.red;
        loginErrorText.gameObject.SetActive(false);

        // Bouton de connexion
        loginButton = CreateButton(panel, "LoginButton", "CONNEXION",
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f),
            new Vector2(0, 0), new Vector2(200, 50),
            new Color(0.831f, 0.035f, 0.035f));

        // Bouton pour aller à l'inscription
        toRegisterButton = CreateButton(panel, "ToRegisterButton", "Créer un compte",
            new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f),
            new Vector2(0, 0), new Vector2(200, 40),
            new Color(0.2f, 0.2f, 0.2f));

        return panel;
    }

    private GameObject CreateRegisterPanel()
    {
        // Panel d'inscription
        GameObject panel = CreateBasePanel("RegisterPanel");

        // Titre
        TextMeshProUGUI titleText = CreateText(panel, "TitleText", "CRÉER UN COMPTE", 24,
            new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f),
            new Vector2(0, 0), new Vector2(300, 50));

        // Nom
        TextMeshProUGUI nameLabel = CreateText(panel, "NameLabel", "Nom :", 16,
            new Vector2(0.3f, 0.8f), new Vector2(0.3f, 0.8f),
            new Vector2(0, 0), new Vector2(150, 30));

        registerNameInput = CreateInputField(panel, "NameInput", "Entrez votre nom",
            new Vector2(0.7f, 0.8f), new Vector2(0.7f, 0.8f),
            new Vector2(0, 0), new Vector2(200, 40));

        // Email
        TextMeshProUGUI emailLabel = CreateText(panel, "UsernameLabel", "Username :", 16,
            new Vector2(0.3f, 0.7f), new Vector2(0.3f, 0.7f),
            new Vector2(0, 0), new Vector2(150, 30));

        registerUsernameInput = CreateInputField(panel, "UsernameInput", "Entrez votre pseudo",
            new Vector2(0.7f, 0.7f), new Vector2(0.7f, 0.7f),
            new Vector2(0, 0), new Vector2(200, 40));

        // Mot de passe
        TextMeshProUGUI passwordLabel = CreateText(panel, "PasswordLabel", "Mot de passe :", 16,
            new Vector2(0.3f, 0.6f), new Vector2(0.3f, 0.6f),
            new Vector2(0, 0), new Vector2(150, 30));

        registerPasswordInput = CreateInputField(panel, "PasswordInput", "Entrez votre mot de passe",
            new Vector2(0.7f, 0.6f), new Vector2(0.7f, 0.6f),
            new Vector2(0, 0), new Vector2(200, 40));
        registerPasswordInput.contentType = TMP_InputField.ContentType.Password;

        // Confirmation mot de passe
        TextMeshProUGUI confirmPasswordLabel = CreateText(panel, "ConfirmPasswordLabel", "Confirmer :", 16,
            new Vector2(0.3f, 0.5f), new Vector2(0.3f, 0.5f),
            new Vector2(0, 0), new Vector2(150, 30));

        registerConfirmPasswordInput = CreateInputField(panel, "ConfirmPasswordInput", "Confirmez votre mot de passe",
            new Vector2(0.7f, 0.5f), new Vector2(0.7f, 0.5f),
            new Vector2(0, 0), new Vector2(200, 40));
        registerConfirmPasswordInput.contentType = TMP_InputField.ContentType.Password;

        // Message d'erreur
        registerErrorText = CreateText(panel, "ErrorText", "", 14,
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f),
            new Vector2(0, 0), new Vector2(400, 30));
        registerErrorText.color = Color.red;
        registerErrorText.gameObject.SetActive(false);

        // Bouton d'inscription
        registerButton = CreateButton(panel, "RegisterButton", "CRÉER UN COMPTE",
            new Vector2(0.5f, 0.3f), new Vector2(0.5f, 0.3f),
            new Vector2(0, 0), new Vector2(200, 50),
            new Color(0.831f, 0.035f, 0.035f));

        // Bouton pour aller à la connexion
        toLoginButton = CreateButton(panel, "ToLoginButton", "J'ai déjà un compte",
            new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f),
            new Vector2(0, 0), new Vector2(200, 40),
            new Color(0.2f, 0.2f, 0.2f));

        return panel;
    }

    private GameObject CreateMainMenuPanel()
    {
        // Panel principal après connexion
        GameObject panel = CreateBasePanel("MainMenuPanel");

        // Message de bienvenue
        welcomeText = CreateText(panel, "WelcomeText", "Bienvenue !", 24,
            new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f),
            new Vector2(0, 0), new Vector2(400, 50));

        // Bouton Jouer
        playButton = CreateButton(panel, "PlayButton", "JOUER",
            new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f),
            new Vector2(0, 0), new Vector2(250, 60),
            new Color(0.831f, 0.035f, 0.035f));

        // Bouton Déconnexion
        logoutButton = CreateButton(panel, "LogoutButton", "DÉCONNEXION",
            new Vector2(0.5f, 0.3f), new Vector2(0.5f, 0.3f),
            new Vector2(0, 0), new Vector2(250, 60),
            new Color(0.6f, 0.1f, 0.1f));

        return panel;
    }

    private GameObject CreateLoadingPanel()
    {
        GameObject panel = CreateBasePanel("LoadingPanel");

        TextMeshProUGUI loadingText = CreateText(panel, "LoadingText", "Chargement...", 24,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, 0), new Vector2(300, 50));

        return panel;
    }

    private GameObject CreateBasePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(transform, false);

        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.12f, 0.95f);

        return panel;
    }

    private TextMeshProUGUI CreateText(GameObject parent, string name, string text, int fontSize,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;

        return textComponent;
    }

    private TMP_InputField CreateInputField(GameObject parent, string name, string placeholder,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = inputObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        Image inputImage = inputObj.AddComponent<Image>();
        inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Zone de texte
        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(inputObj.transform, false);

        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(5, 5);
        textAreaRect.offsetMax = new Vector2(-5, -5);

        // Texte de l'input
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(textArea.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Left;

        // Placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(textArea.transform, false);

        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 16;
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        placeholderText.alignment = TextAlignmentOptions.Left;

        // Composant Input Field
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.textComponent = textComponent;
        inputField.placeholder = placeholderText;
        inputField.textViewport = textAreaRect;

        // Configurer les couleurs
        ColorBlock colors = inputField.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
        colors.selectedColor = new Color(0.4f, 0.4f, 0.4f);
        inputField.colors = colors;

        return inputField;
    }

    private Button CreateButton(GameObject parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Color color)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(
            Mathf.Min(color.r + 0.1f, 1f),
            Mathf.Min(color.g + 0.1f, 1f),
            Mathf.Min(color.b + 0.1f, 1f),
            color.a);
        colors.pressedColor = new Color(
            Mathf.Max(color.r - 0.1f, 0f),
            Mathf.Max(color.g - 0.1f, 0f),
            Mathf.Max(color.b - 0.1f, 0f),
            color.a);
        button.colors = colors;

        // Texte du bouton
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 18;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;

        return button;
    }

    private void InitializeUI()
    {
        // Login panel
        if (toRegisterButton != null) toRegisterButton.onClick.AddListener(ShowRegisterPanel);
        if (loginErrorText != null) loginErrorText.gameObject.SetActive(false);

        // Register panel
        if (toLoginButton != null) toLoginButton.onClick.AddListener(ShowLoginPanel);
        if (registerErrorText != null) registerErrorText.gameObject.SetActive(false);

        // Main menu panel
        if (logoutButton != null) logoutButton.onClick.AddListener(HandleLogout);

        if (playButton != null)
        {
            Debug.Log("Connexion du bouton Jouer pour démarrer le scénario");
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        Debug.Log("LoginUIManager: Démarrage du scénario");

        HideAllPanels();

        Core.ScenarioManager scenarioManager = FindObjectOfType<Core.ScenarioManager>();
        if (scenarioManager != null)
        {
            Debug.Log("ScenarioManager trouvé, démarrage du scénario");
            scenarioManager.StartScenario();
        }
        else
        {
            Debug.LogError("ScenarioManager non trouvé!");

            MainMenu mainMenu = FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                mainMenu.SendMessage("StartGame", null, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Tentative de démarrage via MainMenu");
            }
        }
    }

    #region Panel Management

    public void HideAllPanels()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }

    public void ShowPanel(GameObject panel)
    {
        PlayButtonSound();

        HideAllPanels();

        if (panel != null)
            panel.SetActive(true);
    }

    public void ShowLoginPanel()
    {
        ShowPanel(loginPanel);
        if (loginUsernameInput != null) loginUsernameInput.text = "";
        if (loginPasswordInput != null) loginPasswordInput.text = "";
        if (loginErrorText != null) loginErrorText.gameObject.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        ShowPanel(registerPanel);
        if (registerNameInput != null) registerNameInput.text = "";
        if (registerUsernameInput != null) registerUsernameInput.text = "";
        if (registerPasswordInput != null) registerPasswordInput.text = "";
        if (registerConfirmPasswordInput != null) registerConfirmPasswordInput.text = "";
        if (registerErrorText != null) registerErrorText.gameObject.SetActive(false);
    }

    public void ShowMainMenuPanel()
    {
        ShowPanel(mainMenuPanel);
        if (welcomeText != null && !string.IsNullOrEmpty(currentUserName))
        {
            welcomeText.text = $"Bienvenue, {currentUserName}!";
        }
    }

    public void ShowLoadingPanel()
    {
        ShowPanel(loadingPanel);
        StartCoroutine(LoadingAnimation());
    }

    public void ShowLoginError(string errorMessage)
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (registerPanel != null) registerPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);

        if (loginErrorText != null)
        {
            loginErrorText.text = errorMessage;
            loginErrorText.gameObject.SetActive(true);
        }

        PlayErrorSound();
    }

    public void ShowRegisterError(string errorMessage)
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (registerPanel != null) registerPanel.SetActive(true);
        if (loadingPanel != null) loadingPanel.SetActive(false);

        if (registerErrorText != null)
        {
            registerErrorText.text = errorMessage;
            registerErrorText.gameObject.SetActive(true);
        }

        PlayErrorSound();
    }

    #endregion

    private void HandleLogout()
    {
        PlayButtonSound();

        // Réinitialiser les informations utilisateur
        currentUserName = "";
        IsLoggedIn = false;

        // Appeler l'événement de déconnexion
        onLogout?.Invoke();

        // Revenir à l'écran de connexion
        ShowLoginPanel();
    }

    private IEnumerator LoadingAnimation()
    {
        // Animation de chargement simple
        TextMeshProUGUI loadingText = loadingPanel?.GetComponentInChildren<TextMeshProUGUI>();
        if (loadingText != null)
        {
            int dots = 0;
            while (loadingPanel.activeSelf)
            {
                dots = (dots + 1) % 4;
                string dotsText = new string('.', dots);
                loadingText.text = $"Chargement{dotsText}";
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    #region Audio

    public void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }

    public void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    #endregion
}