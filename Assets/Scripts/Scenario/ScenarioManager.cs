using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class Task
{
    public string taskName;
    public string description;
    public bool requiresBlockchain;
    public bool isCompleted;
    public UnityEvent onTaskCompleted;

    public Task(string name, string desc, bool requiresBC)
    {
        taskName = name;
        description = desc;
        requiresBlockchain = requiresBC;
        isCompleted = false;
        onTaskCompleted = new UnityEvent();
    }
}

public class ScenarioManager : MonoBehaviour
{
    [Header("Scénario Configuration")] 
    [SerializeField] private string scenarioName = "Maintenance Moteur F1";
    [SerializeField] private string scenarioDescription = "Effectuez la maintenance du moteur de Formule 1 en vérifiant chaque composant.";
    [SerializeField] private float timeLimit = 900f; // 15 minutes

    [Header("Mode")] 
    [SerializeField] private bool blockchainModeEnabled = false;

    [Header("Risk Management")] 
    [SerializeField] private float baseRiskLevel = 25f;
    [SerializeField] private float maxRiskLevel = 100f;
    private float currentRiskLevel;

    [Header("UI References")] 
    [SerializeField] private GameUIManager uiManager;

    [Header("Notification Settings")] 
    [SerializeField] private GameObject currentTaskIndicator;

    private FeedbackUIController feedbackController;

    // Liste privée de tâches - définie dans le script, pas dans l'inspecteur
    private List<Task> tasks = new List<Task>();
    private int completedTasks = 0;

    // Référence au TaskListUI pour afficher la liste des tâches
    private TaskListUI taskListUI;
    
    // Référence au CurrentTaskUpdater
    private CurrentTaskUpdater taskUpdater;

    void Start()
    {
        // Initialiser les tâches programmatiquement
        SetupTasks();

        // Trouver le FeedbackUIController
        feedbackController = FindObjectOfType<FeedbackUIController>();

        // Initialiser le scénario et l'interface
        InitializeScenario();

        // Afficher un message de bienvenue
        if (feedbackController != null)
        {
            feedbackController.ShowTemporaryMessage(
                $"Bienvenue dans le scénario: {scenarioName}. Consultez votre liste de tâches pour commencer.", 5f);
        }

        // Créer l'indicateur de tâche s'il n'existe pas
        if (currentTaskIndicator == null)
        {
            CreateTaskIndicator();
        }
    }

    void InitializeScenario()
    {
        // Trouver ou créer l'UI
        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();

        if (uiManager == null)
        {
            GameObject uiCreator = new GameObject("UI_Creator");
            GameUICreator creator = uiCreator.AddComponent<GameUICreator>();
            uiManager = FindObjectOfType<GameUIManager>();
        }

        // Initialiser l'UI
        if (uiManager != null)
        {
            uiManager.SetScenarioName(scenarioName);
            uiManager.ResetTimer(timeLimit);
            uiManager.UpdateProgress(0);
            uiManager.OnTimeUp += HandleTimeUp;
        }

        // Initialiser le niveau de risque
        currentRiskLevel = blockchainModeEnabled ? baseRiskLevel / 2 : baseRiskLevel;

        // Configurer les stations en fonction du mode
        ConfigureStations();

        // Initialiser l'UI des tâches si disponible
        taskListUI = FindObjectOfType<TaskListUI>();
        if (taskListUI != null)
        {
            taskListUI.Initialize(tasks);
        }
    }

    void ConfigureStations()
    {
        // Configurer les stations QR
        QRScannerStation[] qrStations = FindObjectsOfType<QRScannerStation>();
        foreach (QRScannerStation station in qrStations)
        {
            station.isBlockchainMode = blockchainModeEnabled;
        }

        // Configurer les stations de documentation
        DocumentationStation[] docStations = FindObjectsOfType<DocumentationStation>();
        foreach (DocumentationStation station in docStations)
        {
            station.isBlockchainMode = blockchainModeEnabled;
        }

        // Note: Station de diagnostic temporairement retirée
    }

    // Définition des tâches directement dans le code
    void SetupTasks()
    {
        tasks.Clear();

        tasks.Add(new Task("scanner_aileron", "Scanner l'aileron avant de la F1.", false));
        tasks.Add(new Task("verify_docs_aileron", "Vérifier l'authenticité de l'aileron sur l'ordinateur.", false));

        // // Tâches liées au système d'injection
        // tasks.Add(new Task("scanner_injection", "Scanner le QR code du système d'injection", false));
        // tasks.Add(new Task("verify_docs_injection", "Vérifier la documentation du système d'injection", true));
        //
        // // Tâches liées au système de refroidissement
        // tasks.Add(new Task("scanner_refroidissement", "Scanner le QR code du système de refroidissement", false));
        // tasks.Add(new Task("verify_docs_refroidissement", "Vérifier la documentation du système de refroidissement", false));
        //
        // // Vérification finale
        // tasks.Add(new Task("verification_finale", "Effectuer une vérification finale complète du moteur", true));
    }

    // Obtenir le nom du scénario
    public string GetScenarioName()
    {
        return scenarioName;
    }

    public void CompleteTask(string taskName)
    {
        // Trouver la tâche par son nom
        Task task = tasks.Find(t => t.taskName == taskName);

        if (task != null && !task.isCompleted)
        {
            task.isCompleted = true;
            completedTasks++;

            // Mise à jour du risque
            if (task.requiresBlockchain && !blockchainModeEnabled)
            {
                // Augmenter le risque si une tâche blockchain est faite en mode standard
                currentRiskLevel += 15f;
                Debug.Log($"Risque augmenté à {currentRiskLevel}% (tâche blockchain en mode standard)");

                // Informer l'utilisateur
                if (feedbackController != null)
                {
                    feedbackController.ShowTemporaryMessage(
                        "Attention: Risque augmenté! Cette tâche nécessite le mode Blockchain pour une sécurité optimale.",
                        4f);
                }
            }
            else
            {
                // Réduire le risque sinon
                currentRiskLevel -= 5f;
                Debug.Log($"Risque réduit à {currentRiskLevel}%");
            }

            // Limiter le risque
            currentRiskLevel = Mathf.Clamp(currentRiskLevel, 0f, maxRiskLevel);

            // Mettre à jour l'UI
            float progress = (float)completedTasks / tasks.Count;
            if (uiManager != null)
            {
                uiManager.UpdateProgress(progress);
            }

            // Mettre à jour l'UI de la liste des tâches
            if (taskListUI != null)
            {
                taskListUI.UpdateTaskStatus(taskName, true);
            }

            // Informer l'utilisateur
            if (feedbackController != null)
            {
                feedbackController.ShowTemporaryMessage($"Tâche complétée: {task.description}", 3f);
            }

            // Notifier le CurrentTaskUpdater
            if (taskUpdater != null)
            {
                taskUpdater.NotifyTaskCompleted(taskName);
            }

            Debug.Log($"Tâche '{task.description}' complétée ! ({completedTasks}/{tasks.Count})");

            // Déclencher l'événement spécifique à la tâche
            task.onTaskCompleted?.Invoke();

            // Vérifier si toutes les tâches sont complétées
            if (completedTasks >= tasks.Count)
            {
                ScenarioCompleted();
            }
        }
    }

    void HandleTimeUp()
    {
        Debug.Log("Temps écoulé! Scénario échoué.");
        // Afficher un message à l'écran
        ShowFeedbackMessage("TEMPS ÉCOULÉ - ÉCHEC DU SCÉNARIO",
            "Vous n'avez pas réussi à terminer toutes les tâches de maintenance du moteur dans le temps imparti.",
            false);

        // Ajouter ici votre logique de fin de scénario (échec)
    }

    void ScenarioCompleted()
    {
        Debug.Log("Félicitations! Toutes les tâches sont terminées.");

        // Afficher un message différent selon le niveau de risque final
        if (currentRiskLevel < 30f)
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - EXCELLENT TRAVAIL",
                $"Vous avez terminé toutes les tâches de maintenance du moteur avec un niveau de risque minimal ({currentRiskLevel:0}%).",
                true);
        }
        else if (currentRiskLevel < 50f)
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - BON TRAVAIL",
                $"Vous avez terminé toutes les tâches de maintenance du moteur avec un niveau de risque acceptable ({currentRiskLevel:0}%).",
                true);
        }
        else
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - VIGILANCE REQUISE",
                $"Vous avez terminé toutes les tâches de maintenance du moteur, mais le niveau de risque est élevé ({currentRiskLevel:0}%). " +
                "Utilisez le mode blockchain pour des vérifications plus sûres.", true);
        }

        // Ajouter ici votre logique de fin de scénario (réussite)
    }

    // Méthode pour afficher un message de feedback
    private void ShowFeedbackMessage(string title, string message, bool isSuccess)
    {
        Debug.Log($"{title}: {message}");

        var feedbackUI = FindObjectOfType<FeedbackUIController>();
        if (feedbackUI != null)
        {
            feedbackUI.ShowFeedback(title, message, isSuccess);
        }
    }

    private void CreateTaskIndicator()
    {
        // Vérifier si un indicateur existe déjà
        Transform canvas = GameObject.Find("TaskIndicatorCanvas")?.transform;

        if (canvas == null)
        {
            // Créer un canvas pour l'indicateur
            GameObject canvasObj = new GameObject("TaskIndicatorCanvas");
            Canvas canvasComponent = canvasObj.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = 10;

            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            canvas = canvasObj.transform;
        }

        // Créer le panneau d'indicateur
        GameObject indicatorPanel = new GameObject("CurrentTaskPanel");
        indicatorPanel.transform.SetParent(canvas, false);

        RectTransform rect = indicatorPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(300, 100);

        Image background = indicatorPanel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f);

        // Ajouter le texte de titre
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(indicatorPanel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, 0);
        titleRect.sizeDelta = new Vector2(0, 30);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Tâche actuelle:";
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Left;

        // Ajouter le texte de description
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.SetParent(indicatorPanel.transform, false);

        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.anchoredPosition = new Vector2(0, -15);
        descRect.sizeDelta = new Vector2(-20, -40);

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = "Consultez votre liste de tâches...";
        descText.fontSize = 14;
        descText.color = Color.white;
        descText.alignment = TextAlignmentOptions.Left;

        // Ajouter le script de mise à jour de la tâche
        taskUpdater = indicatorPanel.AddComponent<CurrentTaskUpdater>();
        taskUpdater.Initialize(titleText, descText, this);

        currentTaskIndicator = indicatorPanel;
    }

    // Accesseurs publics
    public float GetRiskLevel()
    {
        return currentRiskLevel;
    }

    public List<Task> GetTasks()
    {
        return tasks;
    }

    public int GetTaskCount()
    {
        return tasks.Count;
    }

    public int GetCompletedTaskCount()
    {
        return completedTasks;
    }

    public bool IsBlockchainModeEnabled()
    {
        return blockchainModeEnabled;
    }

    public void SetBlockchainMode(bool enabled)
    {
        blockchainModeEnabled = enabled;
        ConfigureStations();
    }
}