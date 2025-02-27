using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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
    [SerializeField] private string scenarioName = "Contrôle Qualité F1";
    [SerializeField] private string scenarioDescription = "Vérifiez l'authenticité et la qualité des pièces de Formule 1 avant l'assemblage.";
    [SerializeField] private float timeLimit = 600f; // 10 minutes
    
    [Header("Mode")]
    [SerializeField] private bool blockchainModeEnabled = false;
    
    [Header("Risk Management")]
    [SerializeField] private float baseRiskLevel = 20f;
    [SerializeField] private float maxRiskLevel = 100f;
    private float currentRiskLevel;
    
    [Header("UI References")]
    [SerializeField] private GameUIManager uiManager;
    
    // Liste privée de tâches - définie dans le script, pas dans l'inspecteur
    private List<Task> tasks = new List<Task>();
    private int completedTasks = 0;
    
    // Référence au TaskListUI pour afficher la liste des tâches
    private TaskListUI taskListUI;
    
    void Start()
    {
        // Initialiser les tâches programmatiquement
        SetupTasks();
        
        // Initialiser le scénario et l'interface
        InitializeScenario();
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
    }
    
    // Définition des tâches directement dans le code
    void SetupTasks()
    {
        // Vider la liste au cas où
        tasks.Clear();
        
        // Créer et ajouter les tâches du scénario
        
        // Tâches liées à l'aileron
        tasks.Add(new Task("scanner_piece_aileron", "Scanner le QR code de l'aileron arrière", false));
        tasks.Add(new Task("verify_docs_aileron", "Vérifier la documentation technique de l'aileron", false));
        
        // Tâches liées au système de freinage
        tasks.Add(new Task("scanner_piece_frein", "Scanner le QR code du système de freinage", false));
        tasks.Add(new Task("verify_docs_frein", "Vérifier la documentation du système de freinage", true));
        
        // Tâches liées au volant
        tasks.Add(new Task("scanner_piece_volant", "Scanner le QR code du volant électronique", false));
        tasks.Add(new Task("verify_docs_volant", "Vérifier la documentation du volant électronique", true));
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
            uiManager.UpdateProgress(progress);
            
            // Mettre à jour l'UI de la liste des tâches
            // TaskListUI taskListUI = FindObjectOfType<TaskListUI>();
            // if (taskListUI != null)
            // {
            //     taskListUI.UpdateTaskStatus(taskName, true);
            // }
            
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
            "Vous n'avez pas réussi à terminer toutes les tâches dans le temps imparti.", false);
        
        // Ajouter ici votre logique de fin de scénario (échec)
    }
    
    void ScenarioCompleted()
    {
        Debug.Log("Félicitations! Toutes les tâches sont terminées.");
        
        // Afficher un message différent selon le niveau de risque final
        if (currentRiskLevel < 30f)
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - EXCELLENT TRAVAIL", 
                $"Vous avez terminé toutes les tâches avec un niveau de risque minimal ({currentRiskLevel:0}%).", true);
        }
        else if (currentRiskLevel < 50f)
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - BON TRAVAIL", 
                $"Vous avez terminé toutes les tâches avec un niveau de risque acceptable ({currentRiskLevel:0}%).", true);
        }
        else
        {
            ShowFeedbackMessage("SCÉNARIO RÉUSSI - VIGILANCE REQUISE", 
                $"Vous avez terminé toutes les tâches, mais le niveau de risque est élevé ({currentRiskLevel:0}%). " +
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