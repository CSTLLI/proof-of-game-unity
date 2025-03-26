using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QRScannerUIGenerator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string canvasName = "QRScannerCanvas";
    [SerializeField] private Vector2 panelSize = new Vector2(600, 450);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Opacité à 100%
    [SerializeField] private bool generateOnStart = false;

    private GameObject canvasObject;
    private GameObject panelObject;
    private QRScannerUIController uiController;

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

        // Ajouter les composants nécessaires au Canvas
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // Créer le panel principal
        panelObject = CreatePanel("ScannerPanel", canvasObject.transform, panelSize, panelColor);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero;

        // Créer l'UI Controller et l'attacher au GameObject actuel
        uiController = gameObject.AddComponent<QRScannerUIController>();

        // Créer les éléments UI
        // --- En-tête avec titre et toggle ---
        GameObject headerPanel = CreatePanel("HeaderPanel", panelRect, 
            new Vector2(panelSize.x, 50), 
            new Color(0.05f, 0.05f, 0.05f, 1f));
        headerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, panelSize.y/2 - 25);

        GameObject titleObj = CreateTextMeshPro("TitleText", headerPanel.transform, 
            new Vector2(0, 0),
            new Vector2(200, 40), "Scanner QR Code", 22, TextAlignmentOptions.Center);
        titleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        // Toggle blockchain dans l'en-tête à droite
        GameObject toggleObj = CreateToggle("BlockchainToggle", headerPanel.transform,
            new Vector2(panelSize.x/2 - 100, 0),
            "Mode Blockchain");

        // --- Zone principale d'information ---
        GameObject infoPanel = CreatePanel("InfoPanel", panelRect, 
            new Vector2(panelSize.x - 40, 240), 
            new Color(0.15f, 0.15f, 0.15f, 1f));
        infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40);

        // Informations d'aileron
        GameObject productInfoObj = CreateTextMeshPro("ProductInfoText", infoPanel.transform, 
            new Vector2(0, 0),
            new Vector2(panelSize.x - 60, 240), "", 16, TextAlignmentOptions.Left);
        
        // --- Zone pour les détails blockchain ---
        GameObject blockchainPanel = CreatePanel("BlockchainPanel", panelRect,
            new Vector2(panelSize.x - 40, 70), 
            new Color(0.05f, 0.15f, 0.05f, 1f));
        blockchainPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

        GameObject blockchainInfoObj = CreateTextMeshPro("BlockchainInfoText", blockchainPanel.transform, 
            Vector2.zero,
            new Vector2(panelSize.x - 60, 60), "", 14, TextAlignmentOptions.Left);

        // --- Zone de statut et progression ---
        GameObject statusPanel = CreatePanel("StatusPanel", panelRect, 
            new Vector2(panelSize.x, 40), 
            new Color(0.0f, 0.0f, 0.2f, 1f));
        statusPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -130);

        GameObject statusObj = CreateTextMeshPro("StatusText", statusPanel.transform, 
            Vector2.zero,
            new Vector2(panelSize.x - 40, 30), "Prêt à scanner...", 16, TextAlignmentOptions.Center);

        // --- Zone de boutons ---
        GameObject buttonPanel = CreatePanel("ButtonPanel", panelRect, 
            new Vector2(panelSize.x, 60), 
            new Color(0.1f, 0.1f, 0.1f, 1f));
        buttonPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -panelSize.y/2 + 30);

        // Barre de progression
        GameObject progressBarBg = CreateImage("ProgressBarBackground", buttonPanel.transform,
            new Vector2(0, 15),
            new Vector2(panelSize.x - 60, 8), CreateDefaultSprite(new Color(0.2f, 0.2f, 0.2f)));

        GameObject progressBarFill = CreateImage("ScanProgressBar", progressBarBg.transform, 
            Vector2.zero,
            new Vector2(progressBarBg.GetComponent<RectTransform>().sizeDelta.x, 8), 
            CreateDefaultSprite(Color.green));

        // Configurer le remplissage de la barre de progression
        Image progressFillImage = progressBarFill.GetComponent<Image>();
        progressFillImage.type = Image.Type.Filled;
        progressFillImage.fillMethod = Image.FillMethod.Horizontal;
        progressFillImage.fillOrigin = 0;
        progressFillImage.fillAmount = 0;

        // Bouton fermer
        GameObject closeButtonObj = CreateButton("CloseButton", buttonPanel.transform,
            new Vector2(0, -10),
            new Vector2(150, 30),
            "Fermer");

        uiController.SetupReferences(
            panelObject,
            progressFillImage,
            statusObj.GetComponent<TextMeshProUGUI>(),
            productInfoObj.GetComponent<TextMeshProUGUI>(),
            blockchainPanel,
            blockchainInfoObj.GetComponent<TextMeshProUGUI>(),
            closeButtonObj.GetComponent<Button>(),
            toggleObj.GetComponent<Toggle>(),
            this
        );

        GameObject memorizeBtn = CreateButton("MemorizeButton", buttonPanel.transform,
            new Vector2(-180, -10),
            new Vector2(150, 30),
            "Mémoriser infos");
        memorizeBtn.GetComponent<Button>().onClick.AddListener(() => uiController.MemorizeDocumentationInfo());

        panelObject.SetActive(false);

        Debug.Log("Interface Scanner QR Code générée avec succès.");
    }

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

    private GameObject CreateImage(string name, Transform parent, Vector2 position, Vector2 size, Sprite sprite)
    {
        GameObject imageObj = new GameObject(name);
        imageObj.transform.SetParent(parent, false);

        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = imageObj.AddComponent<Image>();
        image.sprite = sprite;

        return imageObj;
    }

    private GameObject CreateTextMeshPro(string name, Transform parent, Vector2 position, Vector2 size,
        string text, int fontSize, TextAlignmentOptions alignment)
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
        tmp.color = Color.white;

        return textObj;
    }

    public GameObject CreateButton(string name, Transform parent, Vector2 position, Vector2 size, string text)
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

    private GameObject CreateToggle(string name, Transform parent, Vector2 position, string label)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);

        RectTransform rect = toggleObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(160, 20);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(toggleObj.transform, false);

        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchoredPosition = new Vector2(-70, 0);
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
        labelRect.anchoredPosition = new Vector2(10, 0);
        labelRect.sizeDelta = new Vector2(140, 20);

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

    private Sprite CreateDefaultSprite(Color color, bool outline = false)
    {
        int width = 100;
        int height = 100;

        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % width;
            int y = i / width;

            if (outline)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    colors[i] = color;
                }
                else
                {
                    colors[i] = Color.clear;
                }
            }
            else
            {
                colors[i] = color;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}