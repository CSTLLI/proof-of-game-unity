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

    [SerializeField] private Color standardModeColor = new Color(0.0f, 0.8f, 0.0f);
    [SerializeField] private Color blockchainModeColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color errorColor = new Color(0.8f, 0.2f, 0.2f);

    // Données de documentation valides
    private string validSerialNumber = "SF23-AER-058";
    private string validValidationCode = "ESSERE";
    private string validManufactureDate = "2025-02-19";
    private string validAccreditation = "3";

    // Nom de la tâche associée à cette station
    private string associatedTaskName = "verify_docs_aileron";

    // Référence à la station de documentation
    private DocumentationStation currentStation;
    private Coroutine verificationCoroutine;
    private bool isVerifying = false;
    private bool isInitialized = false;
    private bool wasMouseLocked = false;
    private bool wasCursorVisible = false;
    private GameObject clearMemButton = null;

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

            if (value)
            {
                AutoFillFields();
            }
            else
            {
                ClearFields();

                // Vérifier si des données sont mémorisées
                CheckForMemorizedData();
            }

            UpdateInfoText();
        }
    }

    private void ClearFields()
    {
        serialNumberInput.text = "";
        validationCodeInput.text = "";
        manufactureDateInput.text = "";
        accreditationInput.text = "";

        serialNumberInput.interactable = true;
        validationCodeInput.interactable = true;
        manufactureDateInput.interactable = true;
        accreditationInput.interactable = true;
    }

    private void CheckForMemorizedData()
    {
        if (PlayerPrefs.HasKey("DocSerialNumber"))
        {
            serialNumberInput.text = PlayerPrefs.GetString("DocSerialNumber", "");
            validationCodeInput.text = PlayerPrefs.GetString("DocValidationCode", "");
            manufactureDateInput.text = PlayerPrefs.GetString("DocManufactureDate", "");
            accreditationInput.text = PlayerPrefs.GetString("DocAccreditation", "");

            // Créer un bouton pour effacer les données mémorisées si pas déjà présent
            if (clearMemButton == null)
            {
                clearMemButton = CreateClearMemoryButton();
            }

            // Ajouter un message d'info
            StartCoroutine(ShowMemoryInfo());
        }
    }

    private IEnumerator ShowMemoryInfo()
    {
        string originalText = infoText.text;
        Color originalColor = infoText.color;

        infoText.text = "Données mémorisées depuis le scanner QR utilisées. Vous pouvez les modifier si nécessaire.";
        infoText.color = new Color(0.2f, 0.6f, 1f); // Bleu informatif

        yield return new WaitForSeconds(3f);

        infoText.text = originalText;
        infoText.color = originalColor;
    }

    private GameObject CreateClearMemoryButton()
    {
        GameObject buttonParent = closeButton.transform.parent.gameObject;

        GameObject clearBtn = new GameObject("ClearMemoryButton");
        clearBtn.transform.SetParent(buttonParent.transform, false);

        RectTransform rect = clearBtn.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(300, 0);
        rect.sizeDelta = new Vector2(140, 40);

        Image image = clearBtn.AddComponent<Image>();
        image.color = new Color(0.3f, 0.1f, 0.1f); // Rouge foncé

        Button button = clearBtn.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.4f, 0.2f, 0.2f);
        colors.pressedColor = new Color(0.2f, 0.05f, 0.05f);
        button.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(clearBtn.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Effacer mémoire";
        tmp.fontSize = 14;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        button.onClick.AddListener(() =>
        {
            ClearFields();
            PlayerPrefs.DeleteKey("DocSerialNumber");
            PlayerPrefs.DeleteKey("DocValidationCode");
            PlayerPrefs.DeleteKey("DocManufactureDate");
            PlayerPrefs.DeleteKey("DocAccreditation");
            PlayerPrefs.Save();

            Destroy(clearBtn);
            clearMemButton = null;

            verificationResultText.text = "Données mémorisées effacées.";
            verificationResultText.color = standardModeColor;
        });

        return clearBtn;
    }

    private void UpdateInfoText()
    {
        if (currentStation != null && currentStation.isBlockchainMode)
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

        // DisablePlayerControls(true);

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
            // Vérifier si des données sont mémorisées
            CheckForMemorizedData();
        }

        UpdateInfoText();
    }

    private void AutoFillFields()
    {
        serialNumberInput.text = validSerialNumber;
        validationCodeInput.text = validValidationCode;
        manufactureDateInput.text = validManufactureDate;
        accreditationInput.text = validAccreditation;

        // Désactiver les champs en mode blockchain
        serialNumberInput.interactable = !currentStation.isBlockchainMode;
        validationCodeInput.interactable = !currentStation.isBlockchainMode;
        manufactureDateInput.interactable = !currentStation.isBlockchainMode;
        accreditationInput.interactable = !currentStation.isBlockchainMode;

        // Supprimer le bouton clear memory s'il existe
        if (clearMemButton != null)
        {
            Destroy(clearMemButton);
            clearMemButton = null;
        }
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
        // DisablePlayerControls(false);

        if (currentStation != null)
        {
            currentStation.interactionText = currentStation.savedInteractionText;
        }

        // Nettoyer la référence au bouton clear memory
        clearMemButton = null;
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
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();

        // En mode blockchain, le processus est automatique et toujours réussi
        if (currentStation.isBlockchainMode)
        {
            verificationResultText.color = blockchainModeColor;

            // Simule le temps de vérification blockchain
            yield return new WaitForSeconds(verificationDuration / 2); // Plus rapide en blockchain

            // Vérifier si un aileron a été scanné et n'est pas encore validé
            string scannedAileronId = null;
            bool isMonacoAileron = false;

            if (scenarioManager != null)
            {
                foreach (string aileronId in new string[]
                             { "monaco1", "monaco2", "barcelone", "monza", "fake", "damaged" })
                {
                    if (scenarioManager.IsAileronScanned(aileronId) && !scenarioManager.IsAileronValidated(aileronId))
                    {
                        scannedAileronId = aileronId;
                        isMonacoAileron = (aileronId == "monaco1" || aileronId == "monaco2");
                        break;
                    }
                }
            }

            if (scannedAileronId != null)
            {
                // Vérifier si c'est un aileron pour Monaco
                if (isMonacoAileron)
                {
                    // Valider l'aileron scanné
                    verificationResultText.text = "RÉSULTAT: Documentation authentifiée par la blockchain.\n" +
                                                  "Certification: Ferrari S.p.A.\n" +
                                                  "Date de validation: " + System.DateTime.Now.ToString("yyyy-MM-dd") +
                                                  "\n" +
                                                  "Hash de vérification: 0xF3RR4R1d0c73bf6721c87a...\n\n" +
                                                  "Aileron validé avec succès pour le GP de Monaco!";

                    scenarioManager.ValidateAileron(scannedAileronId);
                    currentStation.PlaySuccessSound();
                }
                else
                {
                    // Ce n'est pas un aileron Monaco
                    verificationResultText.text = "ATTENTION: Documentation authentifiée par la blockchain.\n" +
                                                  "Certification: Ferrari S.p.A.\n" +
                                                  "Date de validation: " + System.DateTime.Now.ToString("yyyy-MM-dd") +
                                                  "\n" +
                                                  "Hash de vérification: 0xF3RR4R1d0c73bf6721c87a...\n\n" +
                                                  "Cet aileron n'est PAS compatible avec le GP de Monaco!";

                    verificationResultText.color = new Color(0.8f, 0.6f, 0.0f); // Orange pour avertissement
                    currentStation.PlayErrorSound();
                }
            }
            else
            {
                verificationResultText.text = "RÉSULTAT: Documentation authentifiée par la blockchain.\n" +
                                              "Certification: Ferrari S.p.A.\n" +
                                              "Date de validation: " + System.DateTime.Now.ToString("yyyy-MM-dd") +
                                              "\n" +
                                              "Hash de vérification: 0xF3RR4R1d0c73bf6721c87a...\n\n" +
                                              "Aucun aileron à valider. Veuillez d'abord scanner un aileron.";

                currentStation.PlaySuccessSound();
            }
        }
        else // Mode standard
        {
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

            // Pour ajouter un peu d'aléatoire en mode standard
            bool randomError = Random.value < 0.3f && !allCorrect;

            if (allCorrect)
            {
                verificationResultText.color = standardModeColor;

                // Vérifier si un aileron a été scanné mais pas encore validé
                string scannedAileronId = null;
                bool isMonacoAileron = false;

                if (scenarioManager != null)
                {
                    foreach (string aileronId in new string[]
                                 { "monaco1", "monaco2", "barcelone", "monza", "fake", "damaged" })
                    {
                        if (scenarioManager.IsAileronScanned(aileronId) &&
                            !scenarioManager.IsAileronValidated(aileronId))
                        {
                            scannedAileronId = aileronId;
                            isMonacoAileron = (aileronId == "monaco1" || aileronId == "monaco2");
                            break;
                        }
                    }
                }

                if (scannedAileronId != null)
                {
                    if (isMonacoAileron)
                    {
                        verificationResultText.text = "VALIDATION RÉUSSIE: Les données du document sont correctes.\n" +
                                                      "La pièce est authentique et approuvée pour l'utilisation.\n" +
                                                      "Référence: Ferrari " + validSerialNumber + "\n\n" +
                                                      "Aileron validé avec succès pour le GP de Monaco!";

                        scenarioManager.ValidateAileron(scannedAileronId);
                        currentStation.PlaySuccessSound();
                    }
                    else
                    {
                        verificationResultText.text = "ATTENTION: Les données du document sont correctes.\n" +
                                                      "La pièce est authentique et approuvée pour l'utilisation.\n" +
                                                      "Référence: Ferrari " + validSerialNumber + "\n\n" +
                                                      "Cet aileron n'est PAS compatible avec le GP de Monaco!";

                        verificationResultText.color = new Color(0.8f, 0.6f, 0.0f); // Orange pour avertissement
                        currentStation.PlayErrorSound();
                    }
                }
                else
                {
                    verificationResultText.text = "VALIDATION RÉUSSIE: Les données du document sont correctes.\n" +
                                                  "La pièce est authentique et approuvée pour l'utilisation.\n" +
                                                  "Référence: Ferrari " + validSerialNumber + "\n\n" +
                                                  "Aucun aileron à valider. Veuillez d'abord scanner un aileron.";

                    currentStation.PlaySuccessSound();
                }
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

    public void NotifyTaskCompletion(string taskName)
    {
        ScenarioManager scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.CompleteTask(taskName);
        }
    }

    private void ResetUI()
    {
        // Réinitialiser les éléments d'interface
        verificationResultText.text = "En attente de vérification...";
        verificationResultText.color = currentStation != null && currentStation.isBlockchainMode
            ? blockchainModeColor
            : standardModeColor;
    }

    private void Update()
    {
        // Si l'interface est ouverte et qu'on appuie sur Escape, fermer l'interface
        if (documentationPanel != null && documentationPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDocumentation();
        }
    }

    private void DisablePlayerControls(bool disable)
    {
        var scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.DisablePlayerControls(disable);
        }
        else
        {
            // Méthode de secours si le ScenarioManager n'est pas trouvé
            var playerInteractionController = FindObjectOfType<PlayerInteractionController>();
            if (playerInteractionController != null)
            {
                playerInteractionController.enabled = !disable;
                Debug.Log("PlayerInteractionController " + (disable ? "désactivé" : "activé"));
            }
        }
    }

    // Méthode pour définir le nom de la tâche associée à cette station
    public void SetAssociatedTaskName(string taskName)
    {
        associatedTaskName = taskName;
    }
}