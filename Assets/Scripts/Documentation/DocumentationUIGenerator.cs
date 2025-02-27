using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentationUIGenerator : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private string canvasName = "DocumentationCanvas";
    [SerializeField] private Vector2 panelSize = new Vector2(700, 500);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    [SerializeField] private bool generateOnStart = false;

    [Header("Apparence")] 
    [SerializeField] private Color terminalBackgroundColor = new Color(0.05f, 0.05f, 0.05f, 1f);
    [SerializeField] private Color terminalTextColor = new Color(0.0f, 0.8f, 0.0f, 1f); // Vert terminal

    private GameObject canvasObject;
    private GameObject panelObject;
    private DocumentationUIController uiController;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateUI();
        }
    }

    public void GenerateUI()
    {
        // Vérifier si l'UI existe déjà
        if (GameObject.Find(canvasName) != null)
        {
            Debug.LogWarning($"Un Canvas nommé '{canvasName}' existe déjà dans la scène. Génération annulée.");
            return;
        }

        // Créer le Canvas
        canvasObject = new GameObject(canvasName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // S'assurer qu'il est au-dessus des autres UI

        // Ajouter les composants nécessaires au Canvas
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // Créer le panel principal
        panelObject = CreatePanel("DocumentationPanel", canvasObject.transform, panelSize, panelColor);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero; // Centre de l'écran

        // Créer l'UI Controller et l'attacher au GameObject actuel
        uiController = gameObject.AddComponent<DocumentationUIController>();

        // Créer les éléments UI
        GameObject titleObj = CreateTextMeshPro("TitleText", panelRect, new Vector2(0, panelSize.y / 2 - 30),
            new Vector2(panelSize.x - 120, 50), "Terminal de Documentation", 24, TextAlignmentOptions.Center);

        // Créer un panel pour l'écran du terminal avec plus d'espace
        GameObject terminalPanel = CreatePanel("TerminalScreen", panelRect, new Vector2(panelSize.x - 60, panelSize.y - 120), terminalBackgroundColor);
        RectTransform terminalRect = terminalPanel.GetComponent<RectTransform>();
        terminalRect.anchoredPosition = new Vector2(0, 20);

        // Séparateur visuel supérieur
        GameObject separatorTop = CreatePanel("SeparatorTop", terminalRect, 
            new Vector2(terminalRect.sizeDelta.x - 40, 2), new Color(0.3f, 0.3f, 0.3f, 1f));
        separatorTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, terminalRect.sizeDelta.y/2 - 80);

        // Créer la zone de texte d'informations avec plus d'espace
        GameObject infoTextObj = CreateTextMeshPro("InfoText", terminalRect, new Vector2(0, terminalRect.sizeDelta.y/2 - 40),
            new Vector2(terminalRect.sizeDelta.x - 40, 60),
            "Station de vérification de documentation. En attente de saisie de données...", 
            16, TextAlignmentOptions.Left, terminalTextColor);
        
        // Créer la section des champs de saisie avec plus d'espacement vertical
        GameObject inputFieldsContainer = CreatePanel("InputFieldsContainer", terminalRect, 
            new Vector2(terminalRect.sizeDelta.x - 40, 220), new Color(0, 0, 0, 0));
        RectTransform inputFieldsRect = inputFieldsContainer.GetComponent<RectTransform>();
        inputFieldsRect.anchoredPosition = new Vector2(0, -20);

        // Champ de saisie du numéro de série avec espacement amélioré
        GameObject serialNumberLabel = CreateTextMeshPro("SerialNumberLabel", inputFieldsRect, 
            new Vector2(-inputFieldsRect.sizeDelta.x/2 + 120, 90),
            new Vector2(200, 30), "Numéro de série:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject serialNumberField = CreateInputField("SerialNumberInput", inputFieldsRect,
            new Vector2(70, 90), new Vector2(300, 30), "Entrez le numéro de série...");

        // Champ de saisie du code de validation
        GameObject validationCodeLabel = CreateTextMeshPro("ValidationCodeLabel", inputFieldsRect,
            new Vector2(-inputFieldsRect.sizeDelta.x/2 + 120, 40),
            new Vector2(200, 30), "Code de validation:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject validationCodeField = CreateInputField("ValidationCodeInput", inputFieldsRect,
            new Vector2(70, 40), new Vector2(300, 30), "Entrez le code de validation...");

        // Champ de saisie de la date de fabrication
        GameObject manufactureDateLabel = CreateTextMeshPro("ManufactureDateLabel", inputFieldsRect,
            new Vector2(-inputFieldsRect.sizeDelta.x/2 + 120, -10),
            new Vector2(200, 30), "Date de fabrication:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject manufactureDateField = CreateInputField("ManufactureDateInput", inputFieldsRect,
            new Vector2(70, -10), new Vector2(300, 30), "Format: AAAA-MM-JJ");

        // Champ de saisie du niveau d'accréditation
        GameObject accreditationLabel = CreateTextMeshPro("AccreditationLabel", inputFieldsRect,
            new Vector2(-inputFieldsRect.sizeDelta.x/2 + 120, -60),
            new Vector2(200, 30), "Niveau d'accréditation:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject accreditationField = CreateInputField("AccreditationInput", inputFieldsRect,
            new Vector2(70, -60), new Vector2(300, 30), "Entrez le niveau (1-5)");

        // Séparateur visuel inférieur
        GameObject separatorBottom = CreatePanel("SeparatorBottom", terminalRect, 
            new Vector2(terminalRect.sizeDelta.x - 40, 2), new Color(0.3f, 0.3f, 0.3f, 1f));
        separatorBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -terminalRect.sizeDelta.y/2 + 80);

        // Créer la zone de résultats de vérification avec plus d'espace
        GameObject verificationResultObj = CreateTextMeshPro("VerificationResult", terminalRect,
            new Vector2(0, -terminalRect.sizeDelta.y/2 + 40),
            new Vector2(terminalRect.sizeDelta.x - 40, 60),
            "En attente de vérification...", 16, TextAlignmentOptions.Left, terminalTextColor);

        // Créer les boutons avec un meilleur espacement
        GameObject verifyButton = CreateButton("VerifyButton", panelRect,
            new Vector2(-80, -panelSize.y / 2 + 40),
            new Vector2(150, 35),
            "Vérifier");

        GameObject closeButton = CreateButton("CloseButton", panelRect,
            new Vector2(80, -panelSize.y / 2 + 40),
            new Vector2(150, 35),
            "Fermer");

        // Créer le toggle pour le mode blockchain dans un espace dédié en haut à droite
        GameObject toggleContainer = CreatePanel("BlockchainToggleContainer", panelRect,
            new Vector2(120, 40), new Color(0.2f, 0.2f, 0.2f, 0.8f));
        toggleContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelSize.x / 2 - 70, panelSize.y / 2 - 30);
        
        GameObject toggleObj = CreateToggle("BlockchainToggle", toggleContainer.transform,
            new Vector2(0, 0), "Mode Blockchain");

        // Configurer l'UI Controller
        uiController.SetupReferences(
            panelObject,
            infoTextObj.GetComponent<TextMeshProUGUI>(),
            serialNumberField.GetComponent<TMP_InputField>(),
            validationCodeField.GetComponent<TMP_InputField>(),
            manufactureDateField.GetComponent<TMP_InputField>(),
            accreditationField.GetComponent<TMP_InputField>(),
            verificationResultObj.GetComponent<TextMeshProUGUI>(),
            verifyButton.GetComponent<Button>(),
            closeButton.GetComponent<Button>(),
            toggleObj.GetComponent<Toggle>()
        );

        // Cacher le panel au démarrage
        panelObject.SetActive(false);

        Debug.Log("Interface de Documentation générée avec succès.");
    }

    // Méthodes utilitaires pour créer les éléments UI
    private GameObject CreatePanel(string name, Transform parent, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        Image image = panel.AddComponent<Image>();
        image.color = color;

        return panel;
    }

    private GameObject CreateTextMeshPro(string name, Transform parent, Vector2 position, Vector2 size,
        string text, int fontSize, TextAlignmentOptions alignment, Color textColor = default)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = textColor == default ? Color.white : textColor;

        return textObj;
    }

    private GameObject CreateButton(string name, Transform parent, Vector2 position, Vector2 size, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f);

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

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return buttonObj;
    }

    private GameObject CreateInputField(string name, Transform parent, Vector2 position, Vector2 size, string placeholder)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f);

        // Text Area
        GameObject textArea = new GameObject("TextArea");
        textArea.transform.SetParent(inputObj.transform, false);

        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);

        // Input Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(textArea.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = 14;
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
        placeholderText.fontSize = 14;
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        placeholderText.alignment = TextAlignmentOptions.Left;

        // Input Field Component
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.textComponent = textComponent;
        inputField.placeholder = placeholderText;
        inputField.textViewport = textAreaRect;

        // Set colors
        ColorBlock colors = inputField.colors;
        colors.normalColor = new Color(0.1f, 0.1f, 0.1f);
        colors.highlightedColor = new Color(0.2f, 0.2f, 0.2f);
        colors.selectedColor = new Color(0.2f, 0.2f, 0.2f);
        inputField.colors = colors;

        return inputObj;
    }

    private GameObject CreateToggle(string name, Transform parent, Vector2 position, string label)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);

        RectTransform rect = toggleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(120, 20);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(toggleObj.transform, false);

        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(-50, 0);
        bgRect.sizeDelta = new Vector2(20, 20);

        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);

        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);

        RectTransform checkRect = checkmark.AddComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.sizeDelta = new Vector2(-4, -4);
        checkRect.anchoredPosition = Vector2.zero;

        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.8f, 0.2f);

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleObj.transform, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(20, 0);
        labelRect.sizeDelta = new Vector2(100, 20);

        TextMeshProUGUI tmp = labelObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 12;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;

        // Configure toggle
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        toggle.isOn = false;

        return toggleObj;
    }
}