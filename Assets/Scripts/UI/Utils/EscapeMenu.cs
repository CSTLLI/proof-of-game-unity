using Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private GameObject escapeMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    
    private ScenarioManager scenarioManager;
    private bool isPaused = false;
    
    void Start()
    {
        scenarioManager = FindObjectOfType<ScenarioManager>();
        
        if (escapeMenuPanel == null)
        {
            CreateEscapeMenuUI();
        }
        
        escapeMenuPanel.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && scenarioManager && scenarioManager.scenarioInProgress)
        {
            TogglePauseMenu();
        }
    }
    
    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        escapeMenuPanel.SetActive(isPaused);
        
        if (isPaused)
        {
            Time.timeScale = 0; // Mettre en pause
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (scenarioManager != null)
            {
                scenarioManager.DisablePlayerControls(true);
            }
        }
        else
        {
            Time.timeScale = 1; // Reprendre
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (scenarioManager != null)
            {
                scenarioManager.DisablePlayerControls(false);
            }
        }
    }
    
    void CreateEscapeMenuUI()
    {
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        Transform parentTransform;
        
        if (existingCanvas != null && existingCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            parentTransform = existingCanvas.transform;
        }
        else
        {
            GameObject canvasObj = new GameObject("EscapeMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // S'assurer qu'il est au-dessus des autres UI
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();
            parentTransform = canvasObj.transform;
        }
        
        GameObject panel = new GameObject("EscapeMenuPanel");
        panel.transform.SetParent(parentTransform, false);
        escapeMenuPanel = panel;
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 300);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Titre
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 50);
        titleRect.anchoredPosition = new Vector2(0, -25);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Menu";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Bouton Reprendre
        resumeButton = CreateButton("Reprendre", new Vector2(0, 60), panel.transform);
        resumeButton.onClick.AddListener(OnResumeClicked);
        
        // Bouton Menu Principal
        mainMenuButton = CreateButton("Menu Principal", new Vector2(0, 0), panel.transform);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        // Bouton Quitter
        quitButton = CreateButton("Quitter", new Vector2(0, -60), panel.transform);
        quitButton.onClick.AddListener(OnQuitClicked);
    }
    
    Button CreateButton(string text, Vector2 position, Transform parent)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200, 40);
        buttonRect.anchoredPosition = position;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f);
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f);
        button.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        return button;
    }
    
    void OnResumeClicked()
    {
        TogglePauseMenu();
    }
    
    void OnMainMenuClicked()
    {
        Time.timeScale = 1;
        
        GameObject menuPanel = GameObject.Find("MenuCanvas");
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            
            escapeMenuPanel.SetActive(false);
            isPaused = false;
            
            if (scenarioManager != null)
            {
                scenarioManager.scenarioInProgress = false;
                scenarioManager.DisablePlayerControls(true);
            }
        }
        else
        {
            var gameManager = FindObjectOfType<Core.GameManager>();
            if (gameManager != null)
            {
                gameManager.ReturnToMainMenu();
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    
    void OnQuitClicked()
    {
        var gameManager = FindObjectOfType<Core.GameManager>();
        if (gameManager != null)
        {
            gameManager.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}