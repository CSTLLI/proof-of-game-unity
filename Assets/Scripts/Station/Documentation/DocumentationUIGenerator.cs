using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentationUIGenerator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string canvasName = "DocumentationCanvas";
    [SerializeField] private Vector2 panelSize = new Vector2(800, 600);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Opacité à 100%
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
        canvas.sortingOrder = 100;
        
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // Créer le panel principal
        panelObject = CreatePanel("DocumentationPanel", canvasObject.transform, panelSize, panelColor);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;

        // Créer l'UI Controller et l'attacher au GameObject actuel
        uiController = gameObject.AddComponent<DocumentationUIController>();

        // --- En-tête avec titre et toggle ---
        GameObject headerPanel = CreatePanel("HeaderPanel", panelRect, 
            new Vector2(panelSize.x, 60), 
            new Color(0.05f, 0.05f, 0.05f, 1f));
        headerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, panelSize.y/2 - 30);

        GameObject titleObj = CreateTextMeshPro("TitleText", headerPanel.transform, 
            new Vector2(0, 0),
            new Vector2(300, 40), "Terminal de Documentation", 24, TextAlignmentOptions.Center);
        titleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        // Toggle blockchain dans l'en-tête à droite
        GameObject toggleObj = CreateToggle("BlockchainToggle", headerPanel.transform,
            new Vector2(panelSize.x/2 - 100, 0),
            "Mode Blockchain");

        // --- Zone d'informations et instructions ---
        GameObject infoPanel = CreatePanel("InfoPanel", panelRect, 
            new Vector2(panelSize.x - 40, 60), 
            new Color(0.08f, 0.08f, 0.08f, 1f));
        infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, panelSize.y/2 - 90);

        GameObject infoTextObj = CreateTextMeshPro("InfoText", infoPanel.transform, 
            new Vector2(0, 0),
            new Vector2(panelSize.x - 60, 50),
            "Station de vérification de documentation. En attente de saisie de données...", 
            16, TextAlignmentOptions.Center, terminalTextColor);

        // --- Zone principale des champs de saisie ---
        GameObject inputPanel = CreatePanel("InputPanel", panelRect, 
            new Vector2(panelSize.x - 40, 280), 
            terminalBackgroundColor);
        inputPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);

        // Créer les champs de saisie de manière plus responsive
        float fieldSpacing = 55;
        Vector2 labelSize = new Vector2(200, 30);
        Vector2 inputSize = new Vector2(300, 30);
        float startY = 100;

        // Champ 1: Numéro de série
        GameObject serialNumberLabel = CreateTextMeshPro("SerialNumberLabel", inputPanel.transform, 
            new Vector2(-panelSize.x/4 + 30, startY),
            labelSize, "Numéro de série:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject serialNumberField = CreateInputField("SerialNumberInput", inputPanel.transform,
            new Vector2(panelSize.x/4 - 20, startY), inputSize, "Entrez le numéro de série...");

        // Champ 2: Code de validation
        GameObject validationCodeLabel = CreateTextMeshPro("ValidationCodeLabel", inputPanel.transform,
            new Vector2(-panelSize.x/4 + 30, startY - fieldSpacing),
            labelSize, "Code de validation:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject validationCodeField = CreateInputField("ValidationCodeInput", inputPanel.transform,
            new Vector2(panelSize.x/4 - 20, startY - fieldSpacing), inputSize, "Entrez le code de validation...");

        // Champ 3: Date de fabrication
        GameObject manufactureDateLabel = CreateTextMeshPro("ManufactureDateLabel", inputPanel.transform,
            new Vector2(-panelSize.x/4 + 30, startY - fieldSpacing*2),
            labelSize, "Date de fabrication:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject manufactureDateField = CreateInputField("ManufactureDateInput", inputPanel.transform,
            new Vector2(panelSize.x/4 - 20, startY - fieldSpacing*2), inputSize, "Format: AAAA-MM-JJ");

        // Champ 4: Niveau d'accréditation
        GameObject accreditationLabel = CreateTextMeshPro("AccreditationLabel", inputPanel.transform,
            new Vector2(-panelSize.x/4 + 30, startY - fieldSpacing*3),
            labelSize, "Niveau d'accréditation:", 16, TextAlignmentOptions.Left, terminalTextColor);
        
        GameObject accreditationField = CreateInputField("AccreditationInput", inputPanel.transform,
            new Vector2(panelSize.x/4 - 20, startY - fieldSpacing*3), inputSize, "Entrez le niveau (1-5)");

        // --- Zone de résultats de vérification ---
        GameObject resultPanel = CreatePanel("ResultPanel", panelRect, 
            new Vector2(panelSize.x - 40, 120), 
            new Color(0.05f, 0.1f, 0.05f, 1f));
        resultPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -130);

        GameObject verificationResultObj = CreateTextMeshPro("VerificationResult", resultPanel.transform,
            new Vector2(0, 0),
            new Vector2(panelSize.x - 60, 100),
            "En attente de vérification...", 16, TextAlignmentOptions.Left, terminalTextColor);

        // --- Zone de boutons de contrôle ---
        GameObject buttonPanel = CreatePanel("ButtonPanel", panelRect, 
            new Vector2(panelSize.x, 70), 
            new Color(0.1f, 0.1f, 0.1f, 1f));
        buttonPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -panelSize.y/2 + 35);

        // Boutons avec espacement bien défini
        GameObject verifyButton = CreateButton("VerifyButton", buttonPanel.transform,
            new Vector2(-100, 0),
            new Vector2(180, 40),
            "Vérifier");

        GameObject closeButton = CreateButton("CloseButton", buttonPanel.transform,
            new Vector2(100, 0),
            new Vector2(180, 40),
            "Fermer");

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
        tmp.fontSize = 16;
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
        image.color = new Color(0.15f, 0.15f, 0.15f);

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
        colors.normalColor = new Color(0.15f, 0.15f, 0.15f);
        colors.highlightedColor = new Color(0.25f, 0.25f, 0.25f);
        colors.selectedColor = new Color(0.25f, 0.25f, 0.25f);
        inputField.colors = colors;

        return inputObj;
        }

   private GameObject CreateToggle(string name, Transform parent, Vector2 position, string label)
   {
       GameObject toggleObj = new GameObject(name);
       toggleObj.transform.SetParent(parent, false);

       RectTransform rect = toggleObj.AddComponent<RectTransform>();
       rect.anchoredPosition = position;
       rect.sizeDelta = new Vector2(160, 30);

       Toggle toggle = toggleObj.AddComponent<Toggle>();

       // Background
       GameObject background = new GameObject("Background");
       background.transform.SetParent(toggleObj.transform, false);

       RectTransform bgRect = background.AddComponent<RectTransform>();
       bgRect.anchoredPosition = new Vector2(-70, 0);
       bgRect.sizeDelta = new Vector2(24, 24);

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
       labelRect.anchoredPosition = new Vector2(10, 0);
       labelRect.sizeDelta = new Vector2(140, 30);

       TextMeshProUGUI tmp = labelObj.AddComponent<TextMeshProUGUI>();
       tmp.text = label;
       tmp.fontSize = 14;
       tmp.alignment = TextAlignmentOptions.Left;
       tmp.color = Color.white;

       // Configure toggle
       toggle.targetGraphic = bgImage;
       toggle.graphic = checkImage;
       toggle.isOn = false;

       return toggleObj;
   }
}