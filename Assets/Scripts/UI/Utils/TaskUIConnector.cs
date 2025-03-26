using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.UICore;

public class TaskUIConnector : MonoBehaviour
{
    private GameUIManager uiManager;
    
    [SerializeField] private string currentScenarioName = "Scénario de formation";
    [SerializeField] private float scenarioTotalTime = 300f; // 5 minutes
    [SerializeField] private int totalTasks = 10;
    private int completedTasks = 0;
    
    void Start()
    {
        if (FindFirstObjectByType<GameUIManager>() == null)
        {
            GameObject uiCreator = new GameObject("UI_Creator");
            GameUICreator creator = uiCreator.AddComponent<GameUICreator>();
        }
        
        StartCoroutine(FindUIManager());
    }
    
    private IEnumerator FindUIManager()
    {
        yield return null;
        
        uiManager = FindFirstObjectByType<GameUIManager>();
        
        if (uiManager != null)
        {
            uiManager.SetScenarioName(currentScenarioName);
            uiManager.ResetTimer(scenarioTotalTime);
            
            uiManager.OnTimeUp += HandleTimeUp;
        }
        else
        {
            Debug.LogError("GameUIManager introuvable! Assurez-vous que l'UI a été créée.");
        }
    }
    
    public void CompleteTask()
    {
        completedTasks++;
        UpdateProgressUI();
    }
    
    private void UpdateProgressUI()
    {
        if (uiManager != null)
        {
            float progress = totalTasks > 0 ? (float)completedTasks / totalTasks : 0;
            uiManager.UpdateProgress(progress);
        }
    }
    
    private void HandleTimeUp()
    {
        Debug.Log("Le temps est écoulé!");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CompleteTask();
        }
    }
    
    void OnDestroy()
    {
        if (uiManager != null)
        {
            uiManager.OnTimeUp -= HandleTimeUp;
        }
    }
}