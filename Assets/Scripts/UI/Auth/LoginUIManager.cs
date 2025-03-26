using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Core;
using System;

namespace UI.Menu
{
    public class LoginUIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject scenarioSelectionPanel;
        [SerializeField] private GameObject loadingPanel;
        
        [Header("Login Panel")]
        [SerializeField] private TMP_InputField loginEmailInput;
        [SerializeField] private TMP_InputField loginPasswordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button toRegisterButton;
        [SerializeField] private TextMeshProUGUI loginErrorText;
        
        [Header("Register Panel")]
        [SerializeField] private TMP_InputField registerNameInput;
        [SerializeField] private TMP_InputField registerEmailInput;
        [SerializeField] private TMP_InputField registerPasswordInput;
        [SerializeField] private TMP_InputField registerConfirmPasswordInput;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button toLoginButton;
        [SerializeField] private TextMeshProUGUI registerErrorText;
        
        [Header("Main Menu Panel")]
        [SerializeField] private TextMeshProUGUI welcomeText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button logoutButton;
        
        [Header("Scenario Selection Panel")]
        [SerializeField] private Button f1ScenarioButton;
        [SerializeField] private Button backToMainButton;
        [SerializeField] private Button startSelectedScenarioButton;
        
        [Header("Audio")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip errorSound;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem loginEffect;
        
        private AudioSource audioSource;
        private string currentUserName = "";
        private string selectedScenario = "";
        
        // Délégués et événements pour la gestion du login/logout
        public Action onSuccessfulLogin;
        public event Action onLogout;
        public Action onStartGame;
        
        // Variable pour savoir si l'utilisateur est connecté
        public bool IsLoggedIn { get; private set; } = false;
        
        // Propriété pour récupérer le nom d'utilisateur
        public string CurrentUserName => currentUserName;
        
        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        void Start()
        {
            // Si les UI n'ont pas été créées, les créer
            if (loginPanel == null)
            {
                CreateUIElements();
            }
            
            // Initialiser les listeners et configurer les UI
            InitializeUI();
            
            // Commencer avec le panel de login
            ShowLoginPanel();
        }
        
        public void CreateUIElements()
        {
            // Créer un canvas parent pour tous les panels
            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10; // S'assurer qu'il est au-dessus des autres UI
                gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                gameObject.AddComponent<GraphicRaycaster>();
            }
            
            // Créer les panels
            loginPanel = CreateLoginPanel();
            registerPanel = CreateRegisterPanel();
            mainMenuPanel = CreateMainMenuPanel();
            scenarioSelectionPanel = CreateScenarioSelectionPanel();
            loadingPanel = CreateLoadingPanel();
            
            // Configurer les relations entre les différents éléments
            InitializeUI();
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
            TextMeshProUGUI emailLabel = CreateText(panel, "EmailLabel", "Email :", 16, 
                new Vector2(0.3f, 0.7f), new Vector2(0.3f, 0.7f),
                new Vector2(0, 0), new Vector2(150, 30));
            
            loginEmailInput = CreateInputField(panel, "EmailInput", "Entrez votre email", 
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
            TextMeshProUGUI emailLabel = CreateText(panel, "EmailLabel", "Email :", 16, 
                new Vector2(0.3f, 0.7f), new Vector2(0.3f, 0.7f),
                new Vector2(0, 0), new Vector2(150, 30));
            
            registerEmailInput = CreateInputField(panel, "EmailInput", "Entrez votre email", 
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
            
            // Bouton Boutique
            storeButton = CreateButton(panel, "StoreButton", "BOUTIQUE", 
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 0), new Vector2(250, 60),
                new Color(0.4f, 0.4f, 0.4f));
            
            // Bouton Paramètres
            settingsButton = CreateButton(panel, "SettingsButton", "PARAMÈTRES", 
                new Vector2(0.5f, 0.3f), new Vector2(0.5f, 0.3f),
                new Vector2(0, 0), new Vector2(250, 60),
                new Color(0.4f, 0.4f, 0.4f));
            
            // Bouton Déconnexion
            logoutButton = CreateButton(panel, "LogoutButton", "DÉCONNEXION", 
                new Vector2(0.5f, 0.1f), new Vector2(0.5f, 0.1f),
                new Vector2(0, 0), new Vector2(250, 60),
                new Color(0.6f, 0.1f, 0.1f));
            
            return panel;
        }
        
        private GameObject CreateScenarioSelectionPanel()
        {
            // Panel de sélection de scénario
            GameObject panel = CreateBasePanel("ScenarioSelectionPanel");
            
            // Titre
            TextMeshProUGUI titleText = CreateText(panel, "TitleText", "SÉLECTION DE SCÉNARIO", 24, 
                new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f),
                new Vector2(0, 0), new Vector2(400, 50));
                
            // Sous-titre
            TextMeshProUGUI subtitleText = CreateText(panel, "SubtitleText", "Choisissez un scénario pour commencer", 18, 
                new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f),
                new Vector2(0, 0), new Vector2(400, 40));
            
            // Liste des scénarios
// Liste des scénarios
            GameObject scenarioList = new GameObject("ScenarioList");
            scenarioList.transform.SetParent(panel.transform, false);
            RectTransform listRect = scenarioList.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0.5f, 0.5f);
            listRect.anchorMax = new Vector2(0.5f, 0.5f);
            listRect.sizeDelta = new Vector2(500, 200);
            listRect.anchoredPosition = Vector2.zero;
            
            // Scénario F1
            f1ScenarioButton = CreateScenarioButton(scenarioList, "F1ScenarioButton", 
                "Scénario F1 - Contrôle Qualité Ferrari", "Identifiez et validez les ailerons pour Monaco",
                new Vector2(0, 70), new Vector2(450, 80));
            
            // Ajouter d'autres scénarios si nécessaire...
            
            // Bouton pour démarrer le scénario sélectionné
            startSelectedScenarioButton = CreateButton(panel, "StartScenarioButton", "JOUER", 
                new Vector2(0.7f, 0.15f), new Vector2(0.7f, 0.15f),
                new Vector2(0, 0), new Vector2(200, 60),
                new Color(0.831f, 0.035f, 0.035f));
            startSelectedScenarioButton.interactable = false; // Désactiver jusqu'à ce qu'un scénario soit sélectionné
            
            // Bouton pour revenir au menu principal
            backToMainButton = CreateButton(panel, "BackToMainButton", "RETOUR", 
                new Vector2(0.3f, 0.15f), new Vector2(0.3f, 0.15f),
                new Vector2(0, 0), new Vector2(200, 60),
                new Color(0.4f, 0.4f, 0.4f));
            
            return panel;
        }
        
        private Button CreateScenarioButton(GameObject parent, string name, string title, string description, 
            Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent.transform, false);
            
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Titre du scénario
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(buttonObj.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.6f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = title;
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Left;
            titleText.color = Color.white;
            titleText.margin = new Vector4(10, 0, 0, 0);
            
            // Description du scénario
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(buttonObj.transform, false);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0);
            descRect.anchorMax = new Vector2(1, 0.6f);
            descRect.sizeDelta = Vector2.zero;
            descRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = description;
            descText.fontSize = 14;
            descText.alignment = TextAlignmentOptions.Left;
            descText.color = new Color(0.8f, 0.8f, 0.8f);
            descText.margin = new Vector4(10, 0, 0, 0);
            
            Button button = buttonObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            colors.selectedColor = new Color(0.4f, 0.1f, 0.1f, 0.8f);
            colors.pressedColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
            button.colors = colors;
            
            return button;
        }
        
        private GameObject CreateLoadingPanel()
        {
            // Panel de chargement
            GameObject panel = CreateBasePanel("LoadingPanel");
            
            // Message de chargement
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
            if (loginButton != null) loginButton.onClick.AddListener(HandleLogin);
            if (toRegisterButton != null) toRegisterButton.onClick.AddListener(ShowRegisterPanel);
            if (loginErrorText != null) loginErrorText.gameObject.SetActive(false);
            
            // Register panel
            if (registerButton != null) registerButton.onClick.AddListener(HandleRegister);
            if (toLoginButton != null) toLoginButton.onClick.AddListener(ShowLoginPanel);
            if (registerErrorText != null) registerErrorText.gameObject.SetActive(false);
            
            // Main menu panel
            if (playButton != null) playButton.onClick.AddListener(ShowScenarioSelection);
            if (storeButton != null) storeButton.onClick.AddListener(HandleStore);
            if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettings);
            if (logoutButton != null) logoutButton.onClick.AddListener(HandleLogout);
            
            // Scenario selection panel
            if (f1ScenarioButton != null) f1ScenarioButton.onClick.AddListener(() => SelectScenario("f1_scenario"));
            if (backToMainButton != null) backToMainButton.onClick.AddListener(ShowMainMenuPanel);
            if (startSelectedScenarioButton != null) startSelectedScenarioButton.onClick.AddListener(HandleStartSelectedScenario);
        }
        
        #region Panel Management
        public void HideAllPanels()
        {
            if (loginPanel != null) loginPanel.SetActive(false);
            if (registerPanel != null) registerPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (scenarioSelectionPanel != null) scenarioSelectionPanel.SetActive(false);
            if (loadingPanel != null) loadingPanel.SetActive(false);
        }
        
        private void ShowPanel(GameObject panel)
        {
            PlayButtonSound();
            
            HideAllPanels();
            
            if (panel != null)
                panel.SetActive(true);
        }
        
public void ShowLoginPanel()
        {
            ShowPanel(loginPanel);
            if (loginEmailInput != null) loginEmailInput.text = "";
            if (loginPasswordInput != null) loginPasswordInput.text = "";
            if (loginErrorText != null) loginErrorText.gameObject.SetActive(false);
        }
        
        public void ShowRegisterPanel()
        {
            ShowPanel(registerPanel);
            if (registerNameInput != null) registerNameInput.text = "";
            if (registerEmailInput != null) registerEmailInput.text = "";
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
        
        public void ShowScenarioSelection()
        {
            PlayButtonSound();
            ShowPanel(scenarioSelectionPanel);
            
            // Réinitialiser l'état des boutons de scénario
            if (f1ScenarioButton != null)
            {
                f1ScenarioButton.interactable = true;
                
                // Réinitialiser la couleur
                ColorBlock colors = f1ScenarioButton.colors;
                colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                f1ScenarioButton.colors = colors;
            }
            
            // Désactiver le bouton de lancement tant qu'aucun scénario n'est sélectionné
            if (startSelectedScenarioButton != null)
            {
                startSelectedScenarioButton.interactable = false;
            }
            
            // Réinitialiser le scénario sélectionné
            selectedScenario = "";
        }
        
        public void ShowLoadingPanel()
        {
            ShowPanel(loadingPanel);
            StartCoroutine(LoadingAnimation());
        }
        
        private void SelectScenario(string scenarioId)
        {
            PlayButtonSound();
            
            // Enregistrer le scénario sélectionné
            selectedScenario = scenarioId;
            
            // Mettre à jour l'apparence du bouton sélectionné
            if (f1ScenarioButton != null && scenarioId == "f1_scenario")
            {
                ColorBlock colors = f1ScenarioButton.colors;
                colors.normalColor = new Color(0.4f, 0.1f, 0.1f, 0.8f);
                f1ScenarioButton.colors = colors;
            }
            
            // Activer le bouton de lancement
            if (startSelectedScenarioButton != null)
            {
                startSelectedScenarioButton.interactable = true;
            }
        }
        #endregion
        
        #region Button Handlers
        private void HandleLogin()
        {
            PlayButtonSound();
            
            if (loginEmailInput == null || loginPasswordInput == null)
            {
                Debug.LogError("Login input fields not assigned!");
                return;
            }
            
            string email = loginEmailInput.text;
            string password = loginPasswordInput.text;
            
            // Valider les entrées
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                if (loginErrorText != null)
                {
                    loginErrorText.text = "Veuillez remplir tous les champs";
                    loginErrorText.gameObject.SetActive(true);
                }
                PlayErrorSound();
                return;
            }
            
            // Dans une implémentation réelle, vous appelleriez votre API ici
            // Pour l'instant, nous allons simuler une connexion réussie
            ShowLoadingPanel();
            StartCoroutine(SimulateLoginProcess(email));
        }
        
        private void HandleRegister()
        {
            PlayButtonSound();
            
            if (registerNameInput == null || registerEmailInput == null || 
                registerPasswordInput == null || registerConfirmPasswordInput == null)
            {
                Debug.LogError("Register input fields not assigned!");
                return;
            }
            
            string name = registerNameInput.text;
            string email = registerEmailInput.text;
            string password = registerPasswordInput.text;
            string confirmPassword = registerConfirmPasswordInput.text;
            
            // Valider les entrées
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                if (registerErrorText != null)
                {
                    registerErrorText.text = "Veuillez remplir tous les champs";
                    registerErrorText.gameObject.SetActive(true);
                }
                PlayErrorSound();
                return;
            }
            
            if (password != confirmPassword)
            {
                if (registerErrorText != null)
                {
                    registerErrorText.text = "Les mots de passe ne correspondent pas";
                    registerErrorText.gameObject.SetActive(true);
                }
                PlayErrorSound();
                return;
            }
            
            // Dans une implémentation réelle, vous appelleriez votre API ici
            // Pour l'instant, nous allons simuler une inscription réussie
            ShowLoadingPanel();
            StartCoroutine(SimulateRegisterProcess(name));
        }
        
        private void HandleStore()
        {
            PlayButtonSound();
            
            Debug.Log("Store button clicked");
        }
        
        private void HandleSettings()
        {
            PlayButtonSound();
            
            Debug.Log("Settings button clicked");
        }
        
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
        
        private void HandleStartSelectedScenario()
        {
            if (string.IsNullOrEmpty(selectedScenario))
            {
                Debug.LogWarning("No scenario selected!");
                return;
            }
            
            PlayButtonSound();
            PlaySuccessSound();
            
            if (loginEffect != null) loginEffect.Play();
            
            // Démarrer le jeu
            StartCoroutine(StartGameWithDelay());
        }
        #endregion
        
        #region Simulations
        private IEnumerator SimulateLoginProcess(string email)
        {
            // Simuler le délai réseau
            yield return new WaitForSeconds(1.5f);
            
            // Extraire le nom de l'email pour le message de bienvenue
            string name = email.Split('@')[0];
            
            // Mettre à jour les informations utilisateur
            currentUserName = name;
            IsLoggedIn = true;
            
            PlaySuccessSound();
            
            // Appeler l'événement de connexion réussie si défini
            if (onSuccessfulLogin != null)
            {
                onSuccessfulLogin.Invoke();
            }
            else
            {
                ShowMainMenuPanel();
            }
        }
        
        private IEnumerator SimulateRegisterProcess(string name)
        {
            // Simuler le délai réseau
            yield return new WaitForSeconds(2f);
            
            // Mettre à jour les informations utilisateur
            currentUserName = name;
            IsLoggedIn = true;
            
            PlaySuccessSound();
            
            // Appeler l'événement de connexion réussie si défini
            if (onSuccessfulLogin != null)
            {
                onSuccessfulLogin.Invoke();
            }
            else
            {
                ShowMainMenuPanel();
            }
        }
        
        private IEnumerator StartGameWithDelay()
        {
            yield return new WaitForSeconds(1f);
            
            // Cacher tous les panneaux de l'UI
            HideAllPanels();
            
            ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.StartScenario();
            }
            else
            {
                Debug.LogError("ScenarioManager not found in scene!");
            }
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
        #endregion
        
        #region Audio
        private void PlayButtonSound()
        {
            if (audioSource != null && buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }
        }
        
        private void PlaySuccessSound()
        {
            if (audioSource != null && successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
        }
        
        private void PlayErrorSound()
        {
            if (audioSource != null && errorSound != null)
            {
                audioSource.PlayOneShot(errorSound);
            }
        }
        #endregion
    }
}