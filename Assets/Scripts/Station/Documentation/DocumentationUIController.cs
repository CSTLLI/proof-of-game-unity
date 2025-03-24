using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Core;
using Station.Documentation;
using Interaction;

public class DocumentationUIController : MonoBehaviour
{
    [Header("UI Components")] private GameObject documentationPanel;
    private TextMeshProUGUI infoText;
    private TMP_InputField serialNumberInput;
    private TMP_InputField validationCodeInput;
    private TMP_InputField manufactureDateInput;
    private TMP_InputField accreditationInput;
    private TextMeshProUGUI verificationResultText;
    private Button verifyButton;
    private Button closeButton;
    private Toggle blockchainToggle;

    [Header("Animation Settings")] [SerializeField]
    private float verificationDuration = 2f;

    [SerializeField] private Color standardModeColor = new Color(0.0f, 0.8f, 0.0f); // Vert terminal
    [SerializeField] private Color blockchainModeColor = new Color(0.2f, 0.8f, 0.3f); // Vert plus vif
    [SerializeField] private Color errorColor = new Color(0.8f, 0.2f, 0.2f); // Rouge pour les erreurs

    private string validSerialNumber = "SF23-AER-058";
    private string validValidationCode = "ESSERE";
    private string validManufactureDate = "2025-02-19";
    private string validAccreditation = "3";

    private string associatedTaskName = "verify_docs_aileron";

    // Référence à la station de documentation
    private DocumentationStation currentStation;
    private Coroutine verificationCoroutine;
    private bool isVerifying = false;
    private bool isInitialized = false;
    private bool wasMouseLocked = false;
    private bool wasCursorVisible = false;

    // Méthode pour initialiser les références (appelée par le générateur)
    public void SetupReferences(
        GameObject panel,
        TextMeshProUGUI info,
        TMP_InputField serialNumber,
        TMP_InputField validationCode,
        TMP_InputField manufactureDate,
        TMP_InputField accreditation,
        TextMeshProUGUI verificationResult,
        Button verify,
        Button close,
        Toggle toggle)
    {
        documentationPanel = panel;
        infoText = info;
        serialNumberInput = serialNumber;
        validationCodeInput = validationCode;
        manufactureDateInput = manufactureDate;
        accreditationInput = accreditation;
        verificationResultText = verificationResult;
        verifyButton = verify;
        closeButton = close;
        blockchainToggle = toggle;

        // Configurer les événements
        if (verifyButton != null)
            verifyButton.onClick.AddListener(VerifyDocumentation);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDocumentation);

        if (blockchainToggle != null)
            blockchainToggle.onValueChanged.AddListener(OnBlockchainToggleChanged);

        isInitialized = true;
        Debug.Log("DocumentationUIController initialisé avec succès.");
    }

    private void OnBlockchainToggleChanged(bool value)
    {
        if (currentStation != null)
        {
            currentStation.isBlockchainMode = value;

            // Si on est en mode blockchain, auto-remplir les champs
            if (value)
            {
                AutoFillFields();
            }
            else
            {
                ClearFields();
            }

            // Mettre à jour le texte d'état
            UpdateInfoText();
        }
    }

    private void ClearFields()
    {
        serialNumberInput.text = "";
        validationCodeInput.text = "";
        manufactureDateInput.text = "";
        accreditationInput.text = "";

        // Réactiver les champs en mode standard
        serialNumberInput.interactable = true;
        validationCodeInput.interactable = true;
        manufactureDateInput.interactable = true;
        accreditationInput.interactable = true;
    }

    private void UpdateInfoText()
    {
        if (currentStation.isBlockchainMode)
        {
            infoText.text =
                "MODE BLOCKCHAIN: Les données sont automatiquement vérifiées et validées par la blockchain.\nAppuyez sur 'Vérifier' pour confirmer l'authenticité.";
            infoText.color = blockchainModeColor;
        }
        else
        {
            infoText.text =
                "MODE STANDARD: Veuillez saisir manuellement les informations du document à vérifier.\nTous les champs sont obligatoires.";
            infoText.color = standardModeColor;
        }
    }

    public void StartDocumentation(DocumentationStation station)
    {
        if (!isInitialized)
        {
            Debug.LogError(
                "DocumentationUIController n'est pas initialisé. Utilisez SetupReferences avant d'appeler StartDocumentation.");
            return;
        }

        // Sauvegarder l'état actuel du curseur
        wasMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        wasCursorVisible = Cursor.visible;

        // Activer et montrer le curseur
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DisableCameraControl(true);

        currentStation = station;
        if (blockchainToggle != null)
            blockchainToggle.isOn = station.isBlockchainMode;

        documentationPanel.SetActive(true);
        ResetUI();

        // Si c'est en mode blockchain, auto-remplir les champs
        if (station.isBlockchainMode)
        {
            AutoFillFields();
        }
        else
        {
            ClearFields();
        }

        UpdateInfoText();
    }

    public void CloseDocumentation()
    {
        if (verificationCoroutine != null)
            StopCoroutine(verificationCoroutine);

        if (documentationPanel != null)
            documentationPanel.SetActive(false);

        // Restaurer l'état précédent du curseur
        Cursor.lockState = wasMouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = wasCursorVisible;

        // Réactiver le contrôle de la caméra
        DisableCameraControl(false);

        if (currentStation != null)
        {
            currentStation.interactionText = currentStation.savedInteractionText;
        }
    }

    public void VerifyDocumentation()
    {
        if (isVerifying)
            return;

        // Démarrer la vérification
        verificationCoroutine = StartCoroutine(VerificationProcess());
    }

    private IEnumerator VerificationProcess()
    {
        isVerifying = true;
        verificationResultText.text = "Vérification en cours...";

        // Trouver le ScenarioManager
        ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();

        // En mode blockchain, le processus est automatique et toujours réussi
        if (currentStation.isBlockchainMode)
        {
            verificationResultText.color = blockchainModeColor;

            // Simule le temps de vérification blockchain
            yield return new WaitForSeconds(verificationDuration / 2); // Plus rapide en blockchain

            // Vérifier si un aileron a été scanné et n'est pas encore validé
            string scannedAileronId = null;
            if (scenarioManager != null)
            {
                foreach (string aileronId in new string[]
                             { "monaco1", "monaco2", "barcelone", "monza", "fake", "damaged" })
                {
                    if (scenarioManager.IsAileronScanned(aileronId) && !scenarioManager.IsAileronValidated(aileronId))
                    {
                        scannedAileronId = aileronId;
                        break;
                    }
                }
            }

            if (scannedAileronId != null)
            {
                // Valider l'aileron scanné
                verificationResultText.text = "RÉSULTAT: Documentation authentifiée par la blockchain.\n" +
                                              "Certification: Ferrari S.p.A.\n" +
                                              "Date de validation: " + System.DateTime.Now.ToString("yyyy-MM-dd") +
                                              "\n" +
                                              "Hash de vérification: 0xF3RR4R1d0c73bf6721c87a...\n\n" +
                                              "Aileron validé avec succès!";

                scenarioManager.ValidateAileron(scannedAileronId);
            }
            else
            {
                verificationResultText.text = "RÉSULTAT: Documentation authentifiée par la blockchain.\n" +
                                              "Certification: Ferrari S.p.A.\n" +
                                              "Date de validation: " + System.DateTime.Now.ToString("yyyy-MM-dd") +
                                              "\n" +
                                              "Hash de vérification: 0xF3RR4R1d0c73bf6721c87a...\n\n" +
                                              "Aucun aileron à valider. Veuillez d'abord scanner un aileron.";
            }

            currentStation.PlaySuccessSound();
        }
        else
        {
            // Reste du code inchangé pour la vérification manuelle...
            // Vérification manuelle des champs
            bool hasAllFields = !string.IsNullOrEmpty(serialNumberInput.text) &&
                                !string.IsNullOrEmpty(validationCodeInput.text) &&
                                !string.IsNullOrEmpty(manufactureDateInput.text) &&
                                !string.IsNullOrEmpty(accreditationInput.text);

            if (!hasAllFields)
            {
                verificationResultText.color = errorColor;
                verificationResultText.text = "ERREUR: Veuillez remplir tous les champs requis.";
                currentStation.PlayErrorSound();
                isVerifying = false;
                yield break;
            }

            // Simule la durée de vérification manuelle
            yield return new WaitForSeconds(verificationDuration);

            // Vérification des données
            bool correctSerialNumber = serialNumberInput.text == validSerialNumber;
            bool correctValidationCode = validationCodeInput.text == validValidationCode;
            bool correctManufactureDate = manufactureDateInput.text == validManufactureDate;
            bool correctAccreditation = accreditationInput.text == validAccreditation;
            bool allCorrect = correctSerialNumber && correctValidationCode &&
                              correctManufactureDate && correctAccreditation;

            // Pour ajouter un peu d'aléatoire en mode standard (comme dans le QRScanner)
            bool randomError = Random.value < 0.3f && !allCorrect;

            if (allCorrect)
            {
                verificationResultText.color = standardModeColor;

                // Vérifier si un aileron a été scanné mais pas encore validé
                string scannedAileronId = null;
                if (scenarioManager != null)
                {
                    foreach (string aileronId in new string[]
                                 { "monaco1", "monaco2", "barcelone", "monza", "fake", "damaged" })
                    {
                        if (scenarioManager.IsAileronScanned(aileronId) &&
                            !scenarioManager.IsAileronValidated(aileronId))
                        {
                            scannedAileronId = aileronId;
                            break;
                        }
                    }
                }

                if (scannedAileronId != null)
                {
                    verificationResultText.text = "VALIDATION RÉUSSIE: Les données du document sont correctes.\n" +
                                                  "La pièce est authentique et approuvée pour l'utilisation.\n" +
                                                  "Référence: Ferrari " + validSerialNumber + "\n\n" +
                                                  "Aileron validé avec succès!";

                    scenarioManager.ValidateAileron(scannedAileronId);
                }
                else
                {
                    verificationResultText.text = "VALIDATION RÉUSSIE: Les données du document sont correctes.\n" +
                                                  "La pièce est authentique et approuvée pour l'utilisation.\n" +
                                                  "Référence: Ferrari " + validSerialNumber + "\n\n" +
                                                  "Aucun aileron à valider. Veuillez d'abord scanner un aileron.";
                }

                currentStation.PlaySuccessSound();
            }
            else if (randomError)
            {
                // Code existant inchangé
                verificationResultText.color = errorColor;
                verificationResultText.text = "ERREUR DE SYSTÈME: Impossible de compléter la vérification.\n" +
                                              "Le système de vérification standard est actuellement instable.\n" +
                                              "Veuillez réessayer ou utiliser le mode blockchain pour une vérification fiable.";
                currentStation.PlayErrorSound();
            }
            else
            {
                // Code existant inchangé
                verificationResultText.color = errorColor;
                string errorMessage = "ERREUR DE VALIDATION: Les données suivantes sont incorrectes:";

                if (!correctSerialNumber)
                    errorMessage += "\n- Numéro de série invalide";
                if (!correctValidationCode)
                    errorMessage += "\n- Code de validation invalide";
                if (!correctManufactureDate)
                    errorMessage += "\n- Date de fabrication incorrecte";
                if (!correctAccreditation)
                    errorMessage += "\n- Niveau d'accréditation non autorisé";

                errorMessage += "\n\nVeuillez vérifier les informations et réessayer.";
                verificationResultText.text = errorMessage;
                currentStation.PlayErrorSound();
            }
        }

        isVerifying = false;
    }
    
    private void AutoFillFields()
    {
        // Mode blockchain traditionnel
        if (currentStation.isBlockchainMode)
        {
            serialNumberInput.text = validSerialNumber;
            validationCodeInput.text = validValidationCode;
            manufactureDateInput.text = validManufactureDate;
            accreditationInput.text = validAccreditation;
        }
        // Si des infos mémorisées sont disponibles, les utiliser
        else if (PlayerPrefs.HasKey("DocSerialNumber"))
        {
            serialNumberInput.text = PlayerPrefs.GetString("DocSerialNumber", "");
            validationCodeInput.text = PlayerPrefs.GetString("DocValidationCode", "");
            manufactureDateInput.text = PlayerPrefs.GetString("DocManufactureDate", "");
            accreditationInput.text = PlayerPrefs.GetString("DocAccreditation", "");

            // Ajouter un bouton pour effacer ces informations
            GameObject clearBtn = CreateButton("ClearMemButton", documentationPanel.transform,
                new Vector2(200, -documentationPanel.GetComponent<RectTransform>().sizeDelta.y / 2 + 40),
                new Vector2(120, 35),
                "Effacer mémoire");

            clearBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                ClearFields();
                PlayerPrefs.DeleteKey("DocSerialNumber");
                PlayerPrefs.DeleteKey("DocValidationCode");
                PlayerPrefs.DeleteKey("DocManufactureDate");
                PlayerPrefs.DeleteKey("DocAccreditation");
                PlayerPrefs.Save();
                Destroy(clearBtn);
            });
        }

        // Désactiver les champs en mode blockchain
        serialNumberInput.interactable = !currentStation.isBlockchainMode;
        validationCodeInput.interactable = !currentStation.isBlockchainMode;
        manufactureDateInput.interactable = !currentStation.isBlockchainMode;
        accreditationInput.interactable = !currentStation.isBlockchainMode;
    }

// Méthode utilitaire pour créer un bouton
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

    public void NotifyTaskCompletion(string taskName)
    {
        ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.CompleteTask(taskName);
        }
    }

    private void ResetUI()
    {
        // Réinitialiser les éléments d'interface
        verificationResultText.text = "En attente de vérification...";
        verificationResultText.color = currentStation.isBlockchainMode ? blockchainModeColor : standardModeColor;
    }

    private void Update()
    {
        // Si l'interface est ouverte et qu'on appuie sur Escape, fermer l'interface
        if (documentationPanel != null && documentationPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDocumentation();
        }
    }

    private void DisableCameraControl(bool disable)
    {
        // Chercher le contrôleur d'interaction du joueur
        var playerInteractionController = FindFirstObjectByType<PlayerInteractionController>();
        if (playerInteractionController != null)
        {
            playerInteractionController.enabled = !disable;
            Debug.Log("PlayerInteractionController " + (disable ? "désactivé" : "activé"));
        }
    }

    // Méthode pour définir le nom de la tâche associée à cette station
    public void SetAssociatedTaskName(string taskName)
    {
        associatedTaskName = taskName;
    }
}