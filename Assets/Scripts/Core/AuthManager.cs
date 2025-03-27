using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private string apiUrl = "http://localhost:3367/api/auth/login";
    [SerializeField] private string registerUrl = "http://localhost:3367/api/auth/register";
    
    [Header("UI References")]
    public LoginUIManager uiManager;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    void Awake()
    {
        Debug.Log("LoginManager: Awake");
    }
    
    void Start()
    {
        Debug.Log("LoginManager: Start - Initialisation de l'UI...");
        InitializeUI();
    }
    
    void OnEnable()
    {
        Debug.Log("LoginManager: OnEnable - Vérifions les connexions des boutons");
        // Si l'UI n'est pas initialisé dans Start, essayons ici
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<LoginUIManager>();
            Debug.Log($"LoginManager: Recherche de LoginUIManager - Trouvé: {uiManager != null}");
            
            if (uiManager == null)
            {
                Debug.LogError("LoginUIManager introuvable! Assurez-vous qu'il existe dans la scène.");
                return;
            }
        }
        
        Debug.Log("LoginManager: Connexion des événements des boutons...");
        
        // Connecter explicitement les événements de bouton
        if (uiManager.loginButton != null)
        {
            // Vérifions si le bouton est interactif
            if (!uiManager.loginButton.interactable)
            {
                Debug.LogWarning("LoginManager: Le bouton de connexion n'est pas interactif!");
                uiManager.loginButton.interactable = true;
            }
            
            // Utilisons un événement directement attaché plutôt que RemoveAllListeners
            uiManager.loginButton.onClick.RemoveAllListeners();
            uiManager.loginButton.onClick.AddListener(() => {
                Debug.Log("LoginManager: Bouton de connexion cliqué!");
                HandleLogin();
            });
            
            Debug.Log("LoginManager: Bouton de connexion connecté avec succès");
        }
        else
        {
            Debug.LogError("LoginManager: Bouton de connexion non trouvé!");
        }
        
        if (uiManager.registerButton != null)
        {
            uiManager.registerButton.onClick.RemoveAllListeners();
            uiManager.registerButton.onClick.AddListener(() => {
                Debug.Log("LoginManager: Bouton d'inscription cliqué!");
                HandleRegister();
            });
            
            Debug.Log("LoginManager: Bouton d'inscription connecté avec succès");
        }
        else
        {
            Debug.LogWarning("LoginManager: Bouton d'inscription non trouvé!");
        }
        
        // Ajouter une validation directe pour tester les boutons
        Debug.Log("LoginManager: Test de validation des boutons...");
        Debug.Log($"LoginButton != null: {uiManager.loginButton != null}");
        if (uiManager.loginButton != null)
        {
            Debug.Log($"LoginButton interactable: {uiManager.loginButton.interactable}");
            Debug.Log($"LoginButton onClick count: {uiManager.loginButton.onClick.GetPersistentEventCount()}");
        }
    }
    
    // Méthode publique pour tester le bouton directement
    public void TestLoginButton()
    {
        Debug.Log("LoginManager: Test manuel du bouton de connexion");
        HandleLogin();
    }
    
    public void HandleLogin()
    {
        Debug.Log("LoginManager: HandleLogin appelé");
            
        if (uiManager == null)
        {
            Debug.LogError("LoginManager: uiManager est null!");
            return;
        }
        
        if (uiManager.loginUsernameInput == null || uiManager.loginPasswordInput == null)
        {
            Debug.LogError("LoginManager: Champs de connexion non trouvés!");
            return;
        }
        
        string username = uiManager.loginUsernameInput.text;
        string password = uiManager.loginPasswordInput.text;
        
        Debug.Log($"LoginManager: Tentative de connexion avec username: {username}");
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("LoginManager: Champs vides détectés");
            uiManager.ShowLoginError("Veuillez remplir tous les champs");
            return;
        }
            
        uiManager.ShowLoadingPanel();
        StartCoroutine(LoginCoroutine(username, password));
    }
    
    public void HandleRegister()
    {
        Debug.Log("LoginManager: HandleRegister appelé");
            
        if (uiManager == null)
        {
            Debug.LogError("LoginManager: uiManager est null!");
            return;
        }
        
        string username = uiManager.registerNameInput.text;
        string password = uiManager.registerPasswordInput.text;
        string confirmPassword = uiManager.registerConfirmPasswordInput.text;
        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(username) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            uiManager.ShowRegisterError("Veuillez remplir tous les champs");
            return;
        }
        
        if (password != confirmPassword)
        {
            uiManager.ShowRegisterError("Les mots de passe ne correspondent pas");
            return;
        }
            
        uiManager.ShowLoadingPanel();
        StartCoroutine(RegisterCoroutine(username, password));
    }
    
    private IEnumerator LoginCoroutine(string username, string password)
    {
        Debug.Log($"LoginManager: Début de LoginCoroutine pour {username}");
            
        // Create login request with proper field names
        LoginRequest loginRequest = new LoginRequest
        {
            username = username,
            password = password
        };
        
        string jsonData = JsonUtility.ToJson(loginRequest);
        Debug.Log($"LoginManager: Requête: {jsonData}");
        
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
                
            Debug.Log($"LoginManager: Envoi de la requête à {apiUrl}...");
            yield return request.SendWebRequest();
            
            Debug.Log($"LoginManager: Réponse reçue - Code: {request.responseCode}");
            Debug.Log($"LoginManager: Contenu: {request.downloadHandler.text}");
            
            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"LoginManager: Erreur de requête - {request.error}");
                uiManager.ShowLoginError("Erreur de connexion: " + request.error);
            }
            else
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    
                    // Vérifier si la réponse a déjà le champ success
                    if (!responseText.Contains("\"success\":"))
                    {
                        Debug.Log("LoginManager: Adaptation du format de réponse");
                        responseText = "{\"success\":true,\"user\":" + responseText + "}";
                    }
                    
                    Debug.Log($"LoginManager: Réponse traitée: {responseText}");
                    
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);
                    
                    if (response.success)
                    {
                        Debug.Log($"LoginManager: Connexion réussie pour {response.user.username}");
                            
                        // Store user data
                        PlayerPrefs.SetInt("UserID", response.user.id);
                        PlayerPrefs.SetString("Username", response.user.username);
                        if (!string.IsNullOrEmpty(response.token))
                        {
                            PlayerPrefs.SetString("Token", response.token);
                        }
                        PlayerPrefs.Save();
                        
                        // Update UI and play success sound
                        uiManager.currentUserName = response.user.username;
                        uiManager.IsLoggedIn = true;
                        uiManager.PlaySuccessSound();
                        
                        // Show main menu
                        uiManager.ShowMainMenuPanel();
                        
                        // Call success callback if it exists
                        if (uiManager.onSuccessfulLogin != null)
                        {
                            Debug.Log("LoginManager: Appel du callback de succès");
                            uiManager.onSuccessfulLogin.Invoke();
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"LoginManager: Échec de connexion - {response.message}");
                        uiManager.ShowLoginError(response.message ?? "Identifiants invalides");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"LoginManager: Erreur lors du traitement de la réponse - {ex.Message}");
                    Debug.LogError($"Contenu de la réponse: {request.downloadHandler.text}");
                    uiManager.ShowLoginError("Erreur de traitement de la réponse: " + ex.Message);
                }
            }
        }
    }
    
    private IEnumerator RegisterCoroutine(string username, string password)
    {
        Debug.Log($"LoginManager: Début de RegisterCoroutine pour {username}");
            
        // Create register request
        RegisterRequest registerRequest = new RegisterRequest
        {
            username = username,
            password = password
        };
        
        string jsonData = JsonUtility.ToJson(registerRequest);
        
        using (UnityWebRequest request = new UnityWebRequest(registerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            Debug.Log($"LoginManager: Envoi de la requête d'inscription à {registerUrl}...");
            yield return request.SendWebRequest();
            
            Debug.Log($"LoginManager: Réponse d'inscription reçue - Code: {request.responseCode}");
            Debug.Log($"LoginManager: Contenu: {request.downloadHandler.text}");
            
            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"LoginManager: Erreur de requête d'inscription - {request.error}");
                uiManager.ShowRegisterError("Erreur de connexion: " + request.error);
            }
            else
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    
                    // Même logique d'adaptation que pour login
                    if (!responseText.Contains("\"success\":"))
                    {
                        responseText = "{\"success\":true,\"user\":" + responseText + "}";
                    }
                    
                    RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(responseText);
                    
                    if (response.success)
                    {
                        Debug.Log($"LoginManager: Inscription réussie pour {response.user.username}");
                            
                        // Store user data
                        PlayerPrefs.SetInt("UserID", response.user.id);
                        PlayerPrefs.SetString("Username", response.user.username);
                        if (!string.IsNullOrEmpty(response.token))
                        {
                            PlayerPrefs.SetString("Token", response.token);
                        }
                        PlayerPrefs.Save();
                        
                        // Update UI and play success sound
                        uiManager.currentUserName = response.user.username;
                        uiManager.IsLoggedIn = true;
                        uiManager.PlaySuccessSound();
                        
                        // Show main menu
                        uiManager.ShowMainMenuPanel();
                        
                        // Call success callback if it exists
                        if (uiManager.onSuccessfulLogin != null)
                        {
                            uiManager.onSuccessfulLogin.Invoke();
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"LoginManager: Échec d'inscription - {response.message}");
                        uiManager.ShowRegisterError(response.message ?? "Erreur d'inscription");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"LoginManager: Erreur lors du traitement de la réponse d'inscription - {ex.Message}");
                    Debug.LogError($"Contenu de la réponse: {request.downloadHandler.text}");
                    uiManager.ShowRegisterError("Erreur: " + ex.Message);
                }
            }
        }
    }
    
    // Méthode pour être appelée directement dans l'inspecteur ou depuis le code
    public void ConnectUIElements()
    {
        Debug.Log("LoginManager: Connexion manuelle des éléments UI");
        InitializeUI();
    }
}

[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class RegisterRequest
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public UserData user;
    public string token;
}

[System.Serializable]
public class RegisterResponse
{
    public bool success;
    public string message;
    public UserData user;
    public string token;
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
    public string created_at;
    public string subscription_type;
    public string subscription_expiry;
    public bool? subscriptionActive;
}