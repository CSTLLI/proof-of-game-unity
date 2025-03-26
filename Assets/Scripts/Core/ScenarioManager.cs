using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UI.UICore;
using UI.Feedback;
using UI.Modal;
using Scenario;
using UnityEngine.UI;

namespace Core
{
    public class ScenarioManager : MonoBehaviour
    {
        [Header("Scénario Configuration")] [SerializeField]
        private string scenarioName = "Contrôle Qualité - GP Monaco";

        [SerializeField] private float timeLimit = 300f;

        [Header("Progression")] [SerializeField]
        private int requiredAilerons = 2;

        private int aileronsValidated = 0;
        private bool scenarioInProgress = false;
        private float elapsedTime = 0f;

        private Dictionary<string, bool> scannedAilerons = new Dictionary<string, bool>();

        [Header("Mode")] [SerializeField] private bool blockchainModeEnabled = false;

        [Header("Risk Management")] [SerializeField]
        private float baseRiskLevel = 25f;

        [SerializeField] private float maxRiskLevel = 100f;
        private float currentRiskLevel;

        [Header("Camera Control")] [SerializeField]
        private MonoBehaviour playerController;

        [SerializeField] private MonoBehaviour cameraController;

        // Références UI
        private GameUIManager gameUIManager;
        private GameIntroModal introModal;
        private GameEndModal endModal;
        private FeedbackUIController feedbackController;
        
        private ScenarioUIManager uiManager;

        private GameObject taskModal;

        private Dictionary<string, AileronData> ailerons = new Dictionary<string, AileronData>();

        void Awake()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        void Start()
        {
            InitializeAilerons();
            currentRiskLevel = blockchainModeEnabled ? baseRiskLevel / 2 : baseRiskLevel;

            taskModal = GameObject.Find("TaskModal");
            if (taskModal != null)
            {
                taskModal.SetActive(false); // Cacher la modale des tâches au début
            }

            if (playerController == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerController =
                        player.GetComponent<MonoBehaviour>();
                }
            }

            if (cameraController == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraController =
                        mainCamera.GetComponent<MonoBehaviour>();
                }
            }

            DisablePlayerControls(true);

            InitializeUIManager();

            StartCoroutine(ShowIntroAfterDelay());

            if (gameUIManager != null)
            {
                gameUIManager.SetScenarioName(scenarioName);
                gameUIManager.ResetTimer(timeLimit);
                gameUIManager.UpdateProgress(0);
                gameUIManager.OnTimeUp += HandleTimeUp;
                gameUIManager.ToggleTimer(false);
            }

            Debug.Log("ScenarioManager initialisé");
        }

        public void DisablePlayerControls(bool disable)
        {
            var movementController = FindObjectOfType<FirstPersonMovement>();
            if (movementController != null)
                movementController.enabled = !disable;

            if (playerController != null && playerController.gameObject != movementController?.gameObject)
                playerController.enabled = !disable;

            // Désactiver le contrôleur de caméra
            if (cameraController != null)
                cameraController.enabled = !disable;

            // Gérer le curseur
            if (disable)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void StartScenario()
        {
            scenarioInProgress = true;
            elapsedTime = 0f;

            DisablePlayerControls(false);

            if (gameUIManager != null)
            {
                gameUIManager.ResetTimer(timeLimit);
                gameUIManager.ToggleTimer(true);
                gameUIManager.ShowGameUI();

                gameUIManager.ShowTasks(true);
            }

            GameUICreator uiCreator = FindFirstObjectByType<GameUICreator>();
            if (uiCreator != null)
            {
                uiCreator.StartGame();
            }

            StartCoroutine(ShowStartMessageDelayed());

            ConfigureStations();
        }

        private IEnumerator ShowStartMessageDelayed()
        {
            yield return new WaitForSeconds(0.5f);

            if (feedbackController != null)
            {
                feedbackController.ShowTemporaryMessage("Mission commencée! Trouvez les 2 ailerons pour Monaco.", 4f);
            }
            else
            {
                feedbackController = FindFirstObjectByType<FeedbackUIController>();
                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage("Mission commencée! Trouvez les 2 ailerons pour Monaco.",
                        4f);
                }
            }
        }

        private void CompleteScenario(bool success)
        {
            if (!scenarioInProgress) return;

            scenarioInProgress = false;
            DisablePlayerControls(true);

            if (gameUIManager != null)
            {
                gameUIManager.ToggleTimer(false);
                gameUIManager.HideGameUI();
            }

            // taskModal = GameObject.Find("TaskModal");
            // if (taskModal != null)
            // {
            //     taskModal.SetActive(false);
            // }
    
            DocumentationUIController docController = FindObjectOfType<DocumentationUIController>();
            if (docController != null)
            {
                docController.CloseDocumentation();
            }

            GameObject endModalObj = GameObject.Find("EndModal");
            if (endModalObj != null)
            {
                Debug.Log("EndModal trouvé, activation...");
                endModalObj.SetActive(true);
        
                Transform endModalPanelTransform = endModalObj.transform.Find("EndModalPanel");
                if (endModalPanelTransform != null)
                {
                    endModalPanelTransform.gameObject.SetActive(true);
                    Debug.Log("EndModalPanel activé");
                }
        
                GameEndModal endModalComponent = endModalObj.GetComponent<GameEndModal>();
                if (endModalComponent != null)
                {
                    endModalComponent.ShowResults(success, aileronsValidated, requiredAilerons, elapsedTime, currentRiskLevel);
                }
        
                return;
            }
    
            Debug.LogError("EndModal introuvable dans la scène!");
        }

        private void InitializeUIManager()
        {
            uiManager = FindFirstObjectByType<ScenarioUIManager>();

            if (uiManager == null)
            {
                GameObject uiManagerObj = new GameObject("ScenarioUIManager");
                uiManager = uiManagerObj.AddComponent<ScenarioUIManager>();
            }

            uiManager.RegisterScenarioManager(this);

            uiManager.CreateAllUI();

            UpdateUIReferences();
        }

        public void UpdateUIReferences()
        {
            gameUIManager = FindFirstObjectByType<GameUIManager>();
            introModal = FindFirstObjectByType<GameIntroModal>();
            endModal = FindFirstObjectByType<GameEndModal>();
            feedbackController = FindFirstObjectByType<FeedbackUIController>();

            if (gameUIManager != null)
            {
                gameUIManager.SetScenarioName(scenarioName);
                gameUIManager.ResetTimer(timeLimit);
                gameUIManager.UpdateProgress(0);
                gameUIManager.OnTimeUp += HandleTimeUp;
            }

            Debug.Log("Références UI mises à jour: " +
                      "GameUIManager=" + (gameUIManager != null ? "OK" : "NULL") + ", " +
                      "IntroModal=" + (introModal != null ? "OK" : "NULL") + ", " +
                      "EndModal=" + (endModal != null ? "OK" : "NULL"));
        }

        private IEnumerator ShowIntroAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);

            if (introModal == null)
            {
                introModal = FindFirstObjectByType<GameIntroModal>();
            }

            if (introModal != null)
            {
                introModal.Show();
                Debug.Log("Intro modal affiché");
            }
            else
            {
                Debug.LogError("Impossible de trouver ou créer l'intro modal");
            }
        }

        void Update()
        {
            if (scenarioInProgress)
            {
                elapsedTime += Time.deltaTime;

                if (aileronsValidated >= requiredAilerons)
                {
                    CompleteScenario(true);
                }

                if (elapsedTime >= timeLimit)
                {
                    HandleTimeUp();
                }
            }
        }

        private void InitializeAilerons()
        {
            ailerons.Add("monaco1", new AileronData
            {
                name = "SF-MON-01",
                description = "Aileron avant Ferrari Spécial Monaco",
                isForMonaco = true,
                isAuthentic = true
            });

            ailerons.Add("monaco2", new AileronData
            {
                name = "SF-MON-02",
                description = "Aileron avant Ferrari Spécial Monaco - Version légère",
                isForMonaco = true,
                isAuthentic = true
            });

            ailerons.Add("barcelone", new AileronData
            {
                name = "SF-BCN-21",
                description = "Aileron avant Ferrari pour Circuit de Barcelone",
                isForMonaco = false,
                isAuthentic = true
            });

            ailerons.Add("monza", new AileronData
            {
                name = "SF-MNZ-14",
                description = "Aileron avant Ferrari pour Circuit de Monza",
                isForMonaco = false,
                isAuthentic = true
            });

            ailerons.Add("fake", new AileronData
            {
                name = "SF-??-??",
                description = "Aileron suspect - origine inconnue",
                isForMonaco = false,
                isAuthentic = false
            });

            ailerons.Add("damaged", new AileronData
            {
                name = "SF-OLD-07",
                description = "Aileron ancien avec dommages structurels",
                isForMonaco = false,
                isAuthentic = true
            });
        }

        public void ScanAileron(string aileronId)
        {
            if (!ailerons.ContainsKey(aileronId)) return;

            scannedAilerons[aileronId] = false;

            if (feedbackController != null)
            {
                feedbackController.ShowTemporaryMessage(
                    $"Aileron scanné. Complétez la documentation pour valider.", 3f);
            }
        }

        public bool IsAileronScanned(string aileronId)
        {
            return scannedAilerons.ContainsKey(aileronId);
        }

        public bool IsAileronValidated(string aileronId)
        {
            return scannedAilerons.ContainsKey(aileronId) && scannedAilerons[aileronId];
        }

        public void ValidateAileron(string aileronId)
        {
            if (!ailerons.ContainsKey(aileronId)) return;

            if (scannedAilerons.ContainsKey(aileronId) && scannedAilerons[aileronId])
            {
                Debug.Log($"Aileron {aileronId} déjà validé, pas de double validation");
                return;
            }

            scannedAilerons[aileronId] = true;

            AileronData aileron = ailerons[aileronId];

            if (aileron.isForMonaco)
            {
                aileronsValidated++;
                currentRiskLevel -= 10f;

                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage(
                        $"Aileron Monaco validé! ({aileronsValidated}/{requiredAilerons})", 3f);
                }

                if (gameUIManager == null)
                {
                    Debug.LogWarning("gameUIManager est null dans ValidateAileron, tentative de le retrouver...");
                    gameUIManager = FindFirstObjectByType<GameUIManager>();
                }

                if (gameUIManager != null)
                {
                    float progress = (float)aileronsValidated / requiredAilerons;
                    Debug.Log($"Progression du jeu : {progress}");
                    gameUIManager.UpdateProgress(progress);
                }
                else
                {
                    Debug.LogError("Impossible de trouver ou d'utiliser gameUIManager dans ValidateAileron");
                }
            }
            else
            {
                currentRiskLevel += 15f;

                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage("Erreur! Cet aileron n'est pas conçu pour Monaco.", 3f);
                }
            }

            currentRiskLevel = Mathf.Clamp(currentRiskLevel, 0f, maxRiskLevel);
        }

        public void RejectAileron(string aileronId)
        {
            if (!ailerons.ContainsKey(aileronId)) return;

            AileronData aileron = ailerons[aileronId];

            if (aileron.isForMonaco)
            {
                currentRiskLevel += 20f;

                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage("Erreur! Vous avez rejeté un aileron pour Monaco!", 3f);
                }
            }
            else
            {
                currentRiskLevel -= 5f;

                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage("Correct! Cet aileron n'est pas pour Monaco.", 3f);
                }
            }

            currentRiskLevel = Mathf.Clamp(currentRiskLevel, 0f, maxRiskLevel);
        }

        private void HandleTimeUp()
        {
            CompleteScenario(false);
        }

        private void ConfigureStations()
        {
            var qrStations = FindObjectsOfType<Station.QRCode.QRScannerStation>();
            foreach (var station in qrStations)
            {
                station.isBlockchainMode = blockchainModeEnabled;
            }

            var docStations = FindObjectsOfType<Station.Documentation.DocumentationStation>();
            foreach (var station in docStations)
            {
                station.isBlockchainMode = blockchainModeEnabled;
            }
        }

        public void CompleteTask(string taskName)
        {
        }

        public float GetRiskLevel() => currentRiskLevel;
        public string GetScenarioName() => scenarioName;
        public bool IsBlockchainModeEnabled() => blockchainModeEnabled;

        public void SetBlockchainMode(bool enabled)
        {
            blockchainModeEnabled = enabled;

            if (enabled)
                currentRiskLevel = Mathf.Max(currentRiskLevel - 15f, 0f);
            else
                currentRiskLevel = Mathf.Min(currentRiskLevel + 10f, maxRiskLevel);

            ConfigureStations();
        }

        public AileronData GetAileronData(string aileronId)
        {
            Debug.Log("GetAileronData: " + aileronId);

            Debug.Log("Ailerons dictionary contains " + ailerons.Count + " entries");
            foreach (var entry in ailerons)
            {
                Debug.Log($"  Key: '{entry.Key}', Value: {entry.Value.name}");
            }

            if (string.IsNullOrEmpty(aileronId))
            {
                Debug.LogError("GetAileronData: aileronId est null ou vide");
                return null;
            }

            if (!ailerons.ContainsKey(aileronId))
            {
                Debug.LogError($"GetAileronData: aileronId '{aileronId}' n'existe pas dans le dictionnaire");
                return null;
            }

            return ailerons[aileronId];
        }
    }

    public class AileronData
    {
        public string name;
        public string description;
        public bool isForMonaco;
        public bool isAuthentic;
    }
}