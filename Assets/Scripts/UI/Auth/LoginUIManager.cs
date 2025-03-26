using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Core;

namespace UI.Menu
{
    public class LoginUIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject mainMenuPanel;
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
        
        [Header("Audio")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip errorSound;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem loginEffect;
        
        private AudioSource audioSource;
        private string currentUserName = "";
        
        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            
            // Initialize UI elements and set listeners
            InitializeUI();
        }
        
        void Start()
        {
            // Start with login panel active
            ShowLoginPanel();
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
            if (playButton != null) playButton.onClick.AddListener(HandlePlay);
            if (storeButton != null) storeButton.onClick.AddListener(HandleStore);
            if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettings);
            if (logoutButton != null) logoutButton.onClick.AddListener(HandleLogout);
        }
        
        #region Panel Management
        private void ShowPanel(GameObject panel)
        {
            PlayButtonSound();
            
            if (loginPanel != null) loginPanel.SetActive(panel == loginPanel);
            if (registerPanel != null) registerPanel.SetActive(panel == registerPanel);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(panel == mainMenuPanel);
            if (loadingPanel != null) loadingPanel.SetActive(panel == loadingPanel);
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
                welcomeText.text = $"Benvenuto, {currentUserName}!";
            }
        }
        
        public void ShowLoadingPanel()
        {
            ShowPanel(loadingPanel);
            StartCoroutine(LoadingAnimation());
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
            
            // Validate inputs
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
            
            // In a real implementation, you would call your API here
            // For now, we'll simulate a successful login
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
            
            // Validate inputs
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
            
            // In a real implementation, you would call your API here
            // For now, we'll simulate a successful registration
            ShowLoadingPanel();
            StartCoroutine(SimulateRegisterProcess(name));
        }
        
        private void HandlePlay()
        {
            PlayButtonSound();
            PlaySuccessSound();
            
            if (loginEffect != null) loginEffect.Play();
            
            // In a real implementation, you would load the game scene here
            StartCoroutine(StartGameWithDelay());
        }
        
        private void HandleStore()
        {
            PlayButtonSound();
            
            // In a real implementation, you would open the store UI here
            Debug.Log("Store button clicked");
        }
        
        private void HandleSettings()
        {
            PlayButtonSound();
            
            // In a real implementation, you would open the settings UI here
            Debug.Log("Settings button clicked");
        }
        
        private void HandleLogout()
        {
            PlayButtonSound();
            
            // In a real implementation, you would log the user out here
            currentUserName = "";
            ShowLoginPanel();
        }
        #endregion
        
        #region Simulations
        private IEnumerator SimulateLoginProcess(string email)
        {
            // Simulate network delay
            yield return new WaitForSeconds(1.5f);
            
            // Extract name from email for welcome message
            string name = email.Split('@')[0];
            currentUserName = name;
            
            PlaySuccessSound();
            ShowMainMenuPanel();
        }
        
        private IEnumerator SimulateRegisterProcess(string name)
        {
            // Simulate network delay
            yield return new WaitForSeconds(2f);
            
            // Set current user name
            currentUserName = name;
            
            PlaySuccessSound();
            ShowMainMenuPanel();
        }
        
        private IEnumerator StartGameWithDelay()
        {
            yield return new WaitForSeconds(1f);
            
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.StartGame();
            }
            else
            {
                // Fallback if GameManager is not available
                SceneManager.LoadScene("QualityControl");
            }
        }
        
        private IEnumerator LoadingAnimation()
        {
            // Simple loading animation
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