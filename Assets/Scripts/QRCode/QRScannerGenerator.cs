using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QRScannerUIGenerator : MonoBehaviour
{
    [Header("Configuration")] [SerializeField]
    private string canvasName = "QRScannerCanvas";

    [SerializeField] private Vector2 panelSize = new Vector2(600, 400);
    [SerializeField] private Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    [SerializeField] private bool generateOnStart = false;

    [Header("Textures")] [SerializeField] private Sprite scanFrameSprite;
    [SerializeField] private Sprite productIconPlaceholder;

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
        canvas.sortingOrder = 100; // S'assurer qu'il est au-dessus des autres UI

        // Ajouter les composants nécessaires au Canvas
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<GraphicRaycaster>();

        // Créer le panel principal
        panelObject = CreatePanel("ScannerPanel", canvasObject.transform, panelSize, panelColor);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchoredPosition = Vector2.zero; // Centre de l'écran

        // Créer l'UI Controller et l'attacher au GameObject actuel
        uiController = gameObject.AddComponent<QRScannerUIController>();

        // Créer les éléments UI
        GameObject titleObj = CreateTextMeshPro("TitleText", panelRect, new Vector2(0, panelSize.y / 2 - 30),
            new Vector2(panelSize.x, 50), "Scanner QR Code", 24, TextAlignmentOptions.Center);

        // Créer le cadre de scan
        // GameObject frameObj = CreateImage("ScannerFrame", panelRect, Vector2.zero, 
        //     new Vector2(200, 200), scanFrameSprite ? scanFrameSprite : CreateDefaultSprite(Color.white, true));

        // Créer la barre de progression
        GameObject progressBarBg = CreateImage("ProgressBarBackground", panelRect,
            new Vector2(0, -panelSize.y / 2 + 70),
            new Vector2(panelSize.x - 60, 20), CreateDefaultSprite(new Color(0.2f, 0.2f, 0.2f)));

        GameObject progressBarFill = CreateImage("ScanProgressBar", progressBarBg.transform, Vector2.zero,
            new Vector2(progressBarBg.GetComponent<RectTransform>().sizeDelta.x, 20), CreateDefaultSprite(Color.blue));

        // Configurer le remplissage de la barre de progression
        Image progressFillImage = progressBarFill.GetComponent<Image>();
        progressFillImage.type = Image.Type.Filled;
        progressFillImage.fillMethod = Image.FillMethod.Horizontal;
        progressFillImage.fillOrigin = 0;
        progressFillImage.fillAmount = 0;

        // Créer le texte de statut
        GameObject statusObj = CreateTextMeshPro("StatusText", panelRect, new Vector2(0, -panelSize.y / 2 + 100),
            new Vector2(panelSize.x - 40, 30), "Prêt à scanner...", 16, TextAlignmentOptions.Center);

        // Créer la zone d'information produit
        GameObject productInfoObj = CreateTextMeshPro("ProductInfoText", panelRect, new Vector2(0, -50),
            new Vector2(panelSize.x - 60, 120), "", 14, TextAlignmentOptions.Center);

        // Créer le panel pour les détails blockchain
        GameObject blockchainPanel = CreatePanel("BlockchainPanel", panelRect,
            new Vector2(panelSize.x - 80, 150), 
            new Color(0.05f, 0.2f, 0.05f, 0.5f));

        // Centrer le panel
        blockchainPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);

        GameObject blockchainInfoObj = CreateTextMeshPro("BlockchainInfoText", blockchainPanel.transform, Vector2.zero,
            new Vector2(panelSize.x - 100, 100), "", 12, TextAlignmentOptions.Left);

        // Créer l'image du produit
        // GameObject productImageObj = CreateImage("ProductImage", panelRect, new Vector2(panelSize.x / 2 - 80, 50),
        //     new Vector2(100, 100), productIconPlaceholder ? productIconPlaceholder : CreateDefaultSprite(Color.gray));

        // Créer le bouton de fermeture
        GameObject closeButtonObj = CreateButton("CloseButton", panelRect,
            new Vector2(0, -panelSize.y / 2 + 40),
            new Vector2(150, 30),
            "Fermer");

        // Créer le toggle pour le mode blockchain
        GameObject toggleObj = CreateToggle("BlockchainToggle", panelRect,
            new Vector2(panelSize.x / 2 - 80, panelSize.y / 2 - 30),
            "Mode Blockchain");

        // Configurer l'UI Controller
        uiController.SetupReferences(
            panelObject,
            progressFillImage,
            // frameObj.GetComponent<Image>(),
            statusObj.GetComponent<TextMeshProUGUI>(),
            productInfoObj.GetComponent<TextMeshProUGUI>(),
            blockchainPanel,
            blockchainInfoObj.GetComponent<TextMeshProUGUI>(),
            // productImageObj.GetComponent<Image>(),
            closeButtonObj.GetComponent<Button>(),
            toggleObj.GetComponent<Toggle>()
        );

        // Cacher le panel au démarrage
        panelObject.SetActive(false);

        Debug.Log("Interface Scanner QR Code générée avec succès.");
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