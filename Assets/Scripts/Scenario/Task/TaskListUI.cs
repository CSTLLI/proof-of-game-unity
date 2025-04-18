﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Core;
using TMPro;

public class TaskListUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform taskContainer;
    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private TextMeshProUGUI scenarioTitleText;
    [SerializeField] private TextMeshProUGUI riskLevelText;
    
    [Header("Style")]
    [SerializeField] private Color completedTaskColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color pendingTaskColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color blockchainTaskColor = new Color(0.2f, 0.6f, 1f);
    
    // Référence au ScenarioManager
    private ScenarioManager scenarioManager;
    
    // Dictionnaire pour stocker les références aux éléments UI des tâches
    private Dictionary<string, GameObject> taskUIElements = new Dictionary<string, GameObject>();
    
    void Start()
    {
        scenarioManager = FindFirstObjectByType<ScenarioManager>();
        
        if (scenarioManager != null && scenarioTitleText != null)
        {
            scenarioTitleText.text = scenarioManager.name;
        }
    }
    
    void Update()
    {
        if (scenarioManager != null && riskLevelText != null)
        {
            riskLevelText.text = $"Niveau de risque: {scenarioManager.GetRiskLevel():0}%";
            
            float risk = scenarioManager.GetRiskLevel();
            if (risk < 30f)
                riskLevelText.color = Color.green;
            else if (risk < 60f)
                riskLevelText.color = Color.yellow;
            else
                riskLevelText.color = Color.red;
        }
    }
    
    public void Initialize(List<Task> tasks)
    {
        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }
        taskUIElements.Clear();
        
        foreach (Task task in tasks)
        {
            GameObject taskElement = Instantiate(taskPrefab, taskContainer);
            
            // Configurer l'élément UI
            TextMeshProUGUI taskText = taskElement.GetComponentInChildren<TextMeshProUGUI>();
            Toggle taskToggle = taskElement.GetComponentInChildren<Toggle>();
            Image taskImage = taskElement.GetComponent<Image>();
            
            if (taskText != null)
            {
                taskText.text = task.description;
                
                // Marquer les tâches qui nécessitent la blockchain
                if (task.requiresBlockchain)
                {
                    taskText.color = blockchainTaskColor;
                    taskText.text = task.description + " (Blockchain)";
                }
            }
            
            if (taskToggle != null)
            {
                taskToggle.isOn = task.isCompleted;
                taskToggle.interactable = false; // L'utilisateur ne peut pas directement cocher/décocher
            }
            
            taskUIElements[task.taskName] = taskElement;
        }
    }
    
    public void UpdateTaskStatus(string taskName, bool isCompleted)
    {
        if (taskUIElements.TryGetValue(taskName, out GameObject taskElement))
        {
            Toggle taskToggle = taskElement.GetComponentInChildren<Toggle>();
            Image taskBackground = taskElement.GetComponent<Image>();
            
            if (taskToggle != null)
            {
                taskToggle.isOn = isCompleted;
            }
            
            if (taskBackground != null)
            {
                taskBackground.color = isCompleted ? completedTaskColor : pendingTaskColor;
            }
        }
    }
    
    public void ToggleTaskList()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public class Task
    {
        public string taskName;
        public bool isCompleted;
        public string description;
        public bool requiresBlockchain;
    }
}