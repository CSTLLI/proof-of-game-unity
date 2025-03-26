using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Core;
using Interaction;
using Station.QRCode;

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
    private QRScannerUIGenerator scannerGenerator;
    private string currentTaskName = "";
    private Coroutine scanCoroutine;
    private bool isScanning = false;
    private bool isInitialized = false;
    private bool wasMouseLocked = false;
    private bool wasCursorVisible = false;
    
    private MonoBehaviour cameraController;
    private FirstPersonMovement playerMovement;

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
        Toggle toggle,
        QRScannerUIGenerator generator
    )
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
        scannerGenerator = generator;

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

        wasMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        wasCursorVisible = Cursor.visible;
        currentTaskName = taskName;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        Cursor.lockState = wasMouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = wasCursorVisible;

        // Réactiver les contrôles du joueur
        // FindAndDisablePlayerControllers(false);

        if (currentScanner != null)
        {
            currentScanner.interactionText = currentScanner.savedInteractionText;
        }
    }

    private void FindAndDisablePlayerControllers(bool disable)
    {
        // Essayer d'utiliser ScenarioManager d'abord
        var scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.DisablePlayerControls(disable);
        }
        else
        {
            // Méthode de secours - gérer directement les contrôleurs
            if (cameraController == null)
            {
                // Chercher tous les scripts potentiels sur la caméra
                var camControllers = Camera.main?.GetComponents<MonoBehaviour>();
                if (camControllers != null)
                {
                    foreach (var comp in camControllers)
                    {
                        if (comp.GetType().Name.Contains("Look") || comp.GetType().Name.Contains("Camera"))
                        {
                            cameraController = comp;
                            break;
                        }
                    }
                }
            }

            if (playerMovement == null)
            {
                playerMovement = FindObjectOfType<FirstPersonMovement>();
            }

            // Désactiver les contrôleurs
            if (cameraController != null)
                cameraController.enabled = !disable;
                
            if (playerMovement != null)
                playerMovement.enabled = !disable;
        }
    }

    private void Update()
    {
        // Fermer avec Escape
        if (scannerPanel != null && scannerPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseScanner();
        }
    }

    public void DisablePlayerControls(bool disable)
    {
        var scenarioManager = FindFirstObjectByType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.DisablePlayerControls(disable);
        }
    }

    private void ResetUI()
    {
        scanProgressBar.fillAmount = 0f;
        statusText.text = "Initialisation du scan...";
        productInfoText.text = "";
        blockchainPanel.SetActive(false);

        Color modeColor = currentScanner.isBlockchainMode ? blockchainModeColor : standardModeColor;
        scanProgressBar.color = modeColor;

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

        // Trouver les données de l'aileron
        ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
        AileronData aileronData = null;

        if (scenarioManager != null && !string.IsNullOrEmpty(currentTaskName))
        {
            aileronData = scenarioManager.GetAileronData(currentTaskName);
        }

        // Afficher les infos produit de façon plus claire
        if (aileronData != null)
        {
            productInfoText.text = $"<b>{aileronData.name}</b>\n\n{aileronData.description}\n\n" +
                                   $"<color=#FFD700>INFORMATIONS DE DOCUMENTATION :</color>\n" +
                                   $"- Numéro de série : SF23-AER-058\n" +
                                   $"- Code de validation : ESSERE\n" +
                                   $"- Date de fabrication : 2025-02-19\n" +
                                   $"- Niveau d'accréditation : 3";

            blockchainPanel.SetActive(true);

            // Informations blockchain simplifiées
            string status = aileronData.isForMonaco ? "Compatible avec Monaco" : "Non compatible avec Monaco";
            string authenticity = aileronData.isAuthentic ? "Authentique" : "CONTREFAÇON";

            blockchainInfoText.text = $"Statut: {status} | Authenticité: {authenticity}";

            // Marquer l'aileron comme scanné
            if (scenarioManager != null && !string.IsNullOrEmpty(currentTaskName))
            {
                scenarioManager.ScanAileron(currentTaskName);
            }
        }
        else
        {
            // Fallback simplifié
            productInfoText.text = "Aileron F1 Ferrari - Maranello Edition\n\n" +
                                   "Aileron en fibre de carbone de grade aérospatial\n\n" +
                                   "<color=#FFD700>INFORMATIONS DE DOCUMENTATION :</color>\n" +
                                   "- Numéro de série : SF23-AER-058\n" +
                                   "- Code de validation : ESSERE\n" +
                                   "- Date de fabrication : 2025-02-19\n" +
                                   "- Niveau d'accréditation : 3";

            blockchainPanel.SetActive(true);
            blockchainInfoText.text = "Statut: Indéterminé | Origine: Usine Maranello, Italie";
        }

        // Ajouter un message pour guider l'utilisateur
        productInfoText.text += "\n\n<color=#00FFFF>Aileron scanné. Complétez la documentation pour finaliser.</color>";

        // Jouer le son de succès
        currentScanner.PlaySuccessSound();
    }

    private void DisplayStandardResults()
    {
        // Trouver les données de l'aileron
        ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
        AileronData aileronData = null;

        if (scenarioManager != null && !string.IsNullOrEmpty(currentTaskName))
        {
            aileronData = scenarioManager.GetAileronData(currentTaskName);
        }

        // Vérifier si on simule une erreur (40% de chance)
        bool hasError = Random.value < 0.4f;

        if (hasError)
        {
            // Afficher l'erreur
            statusText.text = "Erreur de Vérification";
            statusText.color = Color.red;

            if (aileronData != null)
            {
                productInfoText.text = $"<b>{aileronData.name}</b>\n\n" +
                                       "Impossible de vérifier l'authenticité.\n" +
                                       "Utilisez le mode blockchain pour une vérification complète.";
            }
            else
            {
                productInfoText.text = "Impossible de vérifier l'authenticité de ce composant.\n" +
                                       "Utilisez le mode blockchain pour une vérification complète.";
            }

            // Masquer le panneau blockchain
            blockchainPanel.SetActive(false);

            // Jouer le son d'erreur
            currentScanner.PlayErrorSound();
        }
        else
        {
            // Afficher les infos limitées
            statusText.text = "Scan Terminé - Informations Limitées";
            statusText.color = standardModeColor;

            if (aileronData != null)
            {
                productInfoText.text = $"<b>{aileronData.name}</b>\n\n{aileronData.description}\n\n" +
                                       "Utilisez le mode blockchain pour plus de détails.";

                // Activer le panneau blockchain avec un message d'invitation
                blockchainPanel.SetActive(true);
                blockchainInfoText.text = "Activez le mode blockchain pour vérifier l'authenticité";

                // Marquer l'aileron comme scanné
                if (scenarioManager != null)
                {
                    scenarioManager.ScanAileron(currentTaskName);
                    productInfoText.text +=
                        "\n\n<color=#00FFFF>Aileron scanné. Complétez la documentation pour finaliser.</color>";
                }
            }
            else
            {
                productInfoText.text = "Aileron F1 Ferrari\n\n" +
                                       "Informations limitées en mode standard.\n" +
                                       "Utilisez le mode blockchain pour accéder à l'historique complet.";

                blockchainPanel.SetActive(true);
                blockchainInfoText.text = "Activez le mode blockchain pour vérifier l'authenticité";
            }

            // Jouer le son de succès
            currentScanner.PlaySuccessSound();
        }
    }

    public void MemorizeDocumentationInfo()
    {
        // Version ultra-simplifiée qui se concentre uniquement sur l'enregistrement des données
        PlayerPrefs.SetString("DocSerialNumber", "SF23-AER-058");
        PlayerPrefs.SetString("DocValidationCode", "ESSERE");
        PlayerPrefs.SetString("DocManufactureDate", "2025-02-19");
        PlayerPrefs.SetString("DocAccreditation", "3");
        PlayerPrefs.Save();
    
        Debug.Log("Informations mémorisées avec succès dans PlayerPrefs");
    
        // Notification minimaliste
        if (statusText != null)
        {
            statusText.text = "Informations mémorisées!";
            statusText.color = Color.green;
        
            // Remettre le texte d'origine après un délai
            StartCoroutine(ResetStatusText(2.5f));
        }
    }

    private IEnumerator ResetStatusText(float delay)
    {
        yield return new WaitForSeconds(delay);
    
        if (statusText != null)
        {
            if (currentScanner != null && currentScanner.isBlockchainMode)
            {
                statusText.text = "Scan Réussi - Vérification Blockchain";
                statusText.color = blockchainModeColor;
            }
            else
            {
                statusText.text = "Scan Terminé - Informations Limitées";
                statusText.color = standardModeColor;
            }
        }
    }

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