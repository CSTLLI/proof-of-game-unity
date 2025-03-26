using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarioManager;
    [SerializeField] private Sprite ferrariLogo;

    private GameObject menu;
    
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
    }
    
    private void CreateMenuUI()
    {
        // Créer un canvas
        GameObject canvasObj = new GameObject("MenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Ajouter un fond
        GameObject background = new GameObject("Background");
        background.transform.SetParent(canvasObj.transform, false);
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
            logoObj.transform.SetParent(canvasObj.transform, false);
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
        titleObj.transform.SetParent(canvasObj.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 50);
        titleRect.sizeDelta = new Vector2(600, 100);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "FERRARI F1 - CONTRÔLE QUALITÉ";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.95f, 0.95f, 0.95f);
        titleText.fontStyle = FontStyles.Bold;
        
        // Ajouter un bouton JOUER
        GameObject buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(canvasObj.transform, false);
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
        buttonText.text = "JOUER";
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
    
    private void StartGame()
    {
        menu = GameObject.Find("MenuCanvas");
        if (menu != null)
        {
            menu.SetActive(false);
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