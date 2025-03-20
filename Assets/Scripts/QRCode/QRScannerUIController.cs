using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QRScannerUIController : MonoBehaviour
{
    [Header("UI Components")] private GameObject scannerPanel;
    private Image scanProgressBar;
    private Image scannerFrame;
    private TextMeshProUGUI statusText;
    private TextMeshProUGUI productInfoText;
    private GameObject blockchainPanel;
    private TextMeshProUGUI blockchainInfoText;
    private Image productImage;
    private Button closeButton;
    private Toggle blockchainToggle;

    [Header("Animation Settings")] [SerializeField]
    private float scanDuration = 2f;

    [SerializeField] private Color standardModeColor = new Color(1f, 0.7f, 0.2f); // Orange
    [SerializeField] private Color blockchainModeColor = new Color(0.2f, 0.8f, 0.3f); // Vert

    // Référence au scanner station pour connaître le mode
    private QRScannerStation currentScanner;
    private string currentTaskName = "";
    private Coroutine scanCoroutine;
    private bool isScanning = false;
    private bool isInitialized = false;
    private bool wasMouseLocked = false;
    private bool wasCursorVisible = false;

    private bool isTabletMode = false;
    [SerializeField] private Vector2 tabletPanelSize = new Vector2(450, 350);

    // Méthode pour initialiser les références (appelée par le générateur)
    public void SetupReferences(
        GameObject panel,
        Image progressBar,
        // Image frame,
        TextMeshProUGUI status,
        TextMeshProUGUI productInfo,
        GameObject blockchain,
        TextMeshProUGUI blockchainInfo,
        // Image product,
        Button close,
        Toggle toggle)
    {
        scannerPanel = panel;
        scanProgressBar = progressBar;
        // scannerFrame = frame;
        statusText = status;
        productInfoText = productInfo;
        blockchainPanel = blockchain;
        blockchainInfoText = blockchainInfo;
        // productImage = product;
        closeButton = close;
        blockchainToggle = toggle;

        // Configurer les événements
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseScanner);

        if (blockchainToggle != null)
            blockchainToggle.onValueChanged.AddListener(OnBlockchainToggleChanged);

        float aspectRatio = (float)Screen.width / Screen.height;
        isTabletMode = aspectRatio < 1.5f;

        if (isTabletMode && scannerPanel != null)
        {
            AdjustForTabletMode();
        }

        isInitialized = true;
        Debug.Log("QRScannerUIController initialisé avec succès.");
    }

    private void OnBlockchainToggleChanged(bool value)
    {
        if (currentScanner != null)
        {
            currentScanner.isBlockchainMode = value;

            // Si on n'est pas en train de scanner, mettre à jour l'interface
            if (!isScanning)
            {
                ResetUI();
                // Relancer le scan avec le nouveau mode
                scanCoroutine = StartCoroutine(ScanAnimation());
            }
        }
    }

    public void StartScan(QRScannerStation scanner, string taskName = "")
    {
        if (!isInitialized)
        {
            Debug.LogError(
                "QRScannerUIController n'est pas initialisé. Utilisez SetupReferences avant d'appeler StartScan.");
            return;
        }

        // Sauvegarder l'état actuel du curseur
        wasMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        wasCursorVisible = Cursor.visible;
        currentTaskName = taskName;

        // Activer et montrer le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisableCameraControl(true);

        currentScanner = scanner;
        if (blockchainToggle != null)
            blockchainToggle.isOn = scanner.isBlockchainMode;
        scannerPanel.SetActive(true);
        ResetUI();
        if (scanCoroutine != null)
            StopCoroutine(scanCoroutine);
        scanCoroutine = StartCoroutine(ScanAnimation());
    }

    public void CloseScanner()
    {
        if (scanCoroutine != null)
            StopCoroutine(scanCoroutine);

        if (scannerPanel != null)
            scannerPanel.SetActive(false);

        // Restaurer l'état précédent du curseur
        Cursor.lockState = wasMouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = wasCursorVisible;

        // Réactiver le contrôle de la caméra
        DisableCameraControl(false);

        if (currentScanner != null)
        {
            currentScanner.interactionText = currentScanner.savedInteractionText;
        }
    }

    private void Update()
    {
        // Si l'interface est ouverte et qu'on appuie sur Escape, fermer l'interface
        if (scannerPanel != null && scannerPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseScanner();
        }
    }

    private void DisableCameraControl(bool disable)
    {
        // Chercher le contrôleur d'interaction du joueur
        var playerInteractionController = FindObjectOfType<PlayerInteractionController>();
        if (playerInteractionController != null)
        {
            playerInteractionController.enabled = !disable;
            Debug.Log("PlayerInteractionController " + (disable ? "désactivé" : "activé"));
        }
    }

    private void ResetUI()
    {
        // Réinitialiser les éléments UI
        scanProgressBar.fillAmount = 0f;
        statusText.text = "Initialisation du scan...";
        productInfoText.text = "";
        blockchainPanel.SetActive(false);

        // Configurer les couleurs selon le mode
        Color modeColor = currentScanner.isBlockchainMode ? blockchainModeColor : standardModeColor;
        scanProgressBar.color = modeColor;

        // Réinitialiser l'animation du cadre
        // scannerFrame.transform.localScale = Vector3.one;
    }

    private IEnumerator ScanAnimation()
    {
        isScanning = true;
        float timer = 0f;

        // Animation de la frame du scanner (pulsation)
        // Vector3 originalScale = scannerFrame.transform.localScale;
        // Vector3 targetScale = originalScale * 1.1f;

        // Mise à jour du texte d'état
        statusText.text = "Scan en cours...";

        // Animation de la barre de progression
        while (timer < scanDuration)
        {
            timer += Time.deltaTime;

            // Mise à jour de la barre de progression
            scanProgressBar.fillAmount = timer / scanDuration;

            // Animation de pulsation simple
            float pulse = Mathf.PingPong(Time.time * 2, 1);
            // scannerFrame.transform.localScale = Vector3.Lerp(originalScale, targetScale, pulse);

            yield return null;
        }

        // Scan terminé
        scanProgressBar.fillAmount = 1f;

        // Remettre la frame à sa taille d'origine
        // scannerFrame.transform.localScale = originalScale;

        // Afficher les résultats selon le mode
        if (currentScanner.isBlockchainMode)
        {
            DisplayBlockchainResults();
            currentScanner.PlaySuccessSound();
        }
        else
        {
            DisplayStandardResults();
        }

        isScanning = false;
    }

    private void DisplayBlockchainResults()
    {
        // Configurer l'interface pour les résultats blockchain
        statusText.text = "Scan Réussi - Vérification Blockchain";
        statusText.color = blockchainModeColor;

        // Afficher les infos produit
        productInfoText.text =
            "Aileron F1 Ferrari - Maranello Edition\n\nAileron en fibre de carbone de grade aérospatial, conçu dans les ateliers d'excellence de la Scuderia Ferrari. Performance aérodynamique optimale.";

        // Activer le panneau blockchain
        blockchainPanel.SetActive(true);

        // Afficher les détails blockchain
        blockchainInfoText.text =
            "Certification Ferrari: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
            "\nOrigine: Usine Maranello, Italie" +
            "\nHash de certification: 0xF3RR4R1m4r4n3ll02024c8d73bf6721c87a..." +
            "\nDate de fabrication: 2025-02-19" +
            "\nNuméro de série: SF23-AER-058" +
            "\nValidation: Département Aérodynamique Scuderia Ferrari";
        NotifyTaskCompletion(currentTaskName);
    }

    private void DisplayStandardResults()
    {
        // Vérifier si on simule une erreur (40% de chance comme dans votre script original)
        bool hasError = Random.value < 0.4f;

        if (hasError)
        {
            // Afficher l'erreur
            statusText.text = "Erreur de Vérification - Risque Augmenté";
            statusText.color = Color.red;
            productInfoText.text =
                "Impossible de vérifier l'authenticité de ce composant.\nUtilisez le mode blockchain pour une vérification complète.";

            // Jouer le son d'erreur
            currentScanner.PlayErrorSound();
        }
        else
        {
            // Afficher les infos limitées
            statusText.text = "Scan Terminé - Informations Limitées";
            statusText.color = standardModeColor;
            productInfoText.text =
                "Aileron F1 Ferrari - Maranello Edition\n\nAileron en fibre de carbone de grade aérospatial, conçu dans les ateliers d'excellence de la Scuderia Ferrari. Performance aérodynamique optimale.\nUtilisez le mode blockchain pour accéder à l'historique complet.";

            // Jouer le son de succès
            currentScanner.PlaySuccessSound();
            NotifyTaskCompletion(currentTaskName);
        }
    }

    public void NotifyTaskCompletion(string taskName)
    {
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.CompleteTask(taskName);
        }
    }

// Ajouter cette méthode pour l'ajustement sur tablette
    private void AdjustForTabletMode()
    {
        // Redimensionner le panneau principal
        RectTransform panelRect = scannerPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.sizeDelta = tabletPanelSize;
        }

        // Ajuster les tailles de texte
        if (statusText != null)
        {
            statusText.fontSize -= 2;
        }

        if (productInfoText != null)
        {
            productInfoText.fontSize -= 1;

            // Ajuster la zone de texte
            RectTransform textRect = productInfoText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.sizeDelta = new Vector2(textRect.sizeDelta.x * 0.9f, textRect.sizeDelta.y);
            }
        }

        if (blockchainInfoText != null)
        {
            blockchainInfoText.fontSize -= 1;
        }

        // Redimensionner le panneau blockchain
        if (blockchainPanel != null)
        {
            RectTransform blockchainRect = blockchainPanel.GetComponent<RectTransform>();
            if (blockchainRect != null)
            {
                blockchainRect.sizeDelta = new Vector2(blockchainRect.sizeDelta.x * 0.9f, blockchainRect.sizeDelta.y);
            }
        }

        // Ajuster la position des boutons
        if (closeButton != null)
        {
            RectTransform buttonRect = closeButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.anchoredPosition =
                    new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y + 5);
            }
        }

        if (blockchainToggle != null)
        {
            RectTransform toggleRect = blockchainToggle.GetComponent<RectTransform>();
            if (toggleRect != null)
            {
                toggleRect.anchoredPosition =
                    new Vector2(toggleRect.anchoredPosition.x - 20, toggleRect.anchoredPosition.y);
            }
        }
    }
}