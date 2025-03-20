using UnityEngine;
using TMPro;
using System.Collections;

public class CurrentTaskUpdater : MonoBehaviour
{
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI descriptionText;
    private ScenarioManager scenarioManager;
    
    private string lastTaskName = "";
    private Color standardTaskColor = Color.white;
    private Color blockchainTaskColor = new Color(0.2f, 0.6f, 1f);
    
    public void Initialize(TextMeshProUGUI title, TextMeshProUGUI description, ScenarioManager manager)
    {
        titleText = title;
        descriptionText = description;
        scenarioManager = manager;
        
        // Démarrer la mise à jour périodique
        StartCoroutine(UpdateTaskRoutine());
    }
    
    // Méthode alternative pour l'initialisation sans paramètres
    void Start()
    {
        if (titleText == null)
        {
            titleText = transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        }
        
        if (descriptionText == null)
        {
            descriptionText = transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        }
        
        if (scenarioManager == null)
        {
            scenarioManager = FindObjectOfType<ScenarioManager>();
        }
        
        if (titleText != null && descriptionText != null && scenarioManager != null)
        {
            StartCoroutine(UpdateTaskRoutine());
        }
        else
        {
            Debug.LogError("CurrentTaskUpdater: Des références sont manquantes et n'ont pas pu être trouvées automatiquement.");
        }
    }
    
    IEnumerator UpdateTaskRoutine()
    {
        while (true)
        {
            UpdateCurrentTask();
            yield return new WaitForSeconds(1f); // Vérifier toutes les secondes
        }
    }
    
    void UpdateCurrentTask()
    {
        if (scenarioManager == null || titleText == null || descriptionText == null)
            return;
            
        var tasks = scenarioManager.GetTasks();
        
        // Trouver la première tâche non complétée
        Task nextTask = null;
        foreach (var task in tasks)
        {
            if (!task.isCompleted)
            {
                nextTask = task;
                break;
            }
        }
        
        if (nextTask != null)
        {
            // Ne mettre à jour que si la tâche a changé
            if (lastTaskName != nextTask.taskName)
            {
                titleText.text = "Tâche actuelle:";
                descriptionText.text = nextTask.description;
                
                // Colorer en bleu si blockchain requis
                if (nextTask.requiresBlockchain)
                {
                    descriptionText.color = blockchainTaskColor;
                    
                    // Ajouter un indicateur si le mode blockchain n'est pas activé
                    if (!scenarioManager.IsBlockchainModeEnabled())
                    {
                        descriptionText.text += " (Mode Blockchain requis)";
                    }
                }
                else
                {
                    descriptionText.color = standardTaskColor;
                }
                
                lastTaskName = nextTask.taskName;
            }
        }
        else
        {
            // Toutes les tâches sont complétées
            titleText.text = "Scénario terminé";
            descriptionText.text = "Toutes les tâches ont été complétées!";
            descriptionText.color = Color.green;
        }
    }
    
    // Notification depuis le ScenarioManager lorsqu'une tâche est complétée
    public void NotifyTaskCompleted(string taskName)
    {
        if (lastTaskName == taskName)
        {
            // Forcer la mise à jour immédiate
            lastTaskName = "";
            UpdateCurrentTask();
        }
    }
}