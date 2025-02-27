using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameUIManager : MonoBehaviour
{
    // Références UI
    private TextMeshProUGUI scenarioNameText;
    private TextMeshProUGUI timerText;
    private Slider progressBar;
    
    // Variables de jeu
    private string scenarioName;
    private float totalGameTime;
    private float currentTime;
    private float currentProgress = 0f;
    
    private bool isGameRunning = false;
    
    // Événements
    public event Action OnTimeUp;
    
    public void Initialize(TextMeshProUGUI scenarioText, TextMeshProUGUI timerText, Slider progressBar, 
                          string scenarioName, float totalTime)
    {
        this.scenarioNameText = scenarioText;
        this.timerText = timerText;
        this.progressBar = progressBar;
        this.scenarioName = scenarioName;
        this.totalGameTime = totalTime;
        
        // Configuration initiale
        SetScenarioName(scenarioName);
        currentTime = totalGameTime;
        UpdateTimerDisplay();
        UpdateProgress(0);
        
        // Démarrer automatiquement le jeu
        StartGame();
    }
    
    void Update()
    {
        if (isGameRunning)
        {
            UpdateTimer();
        }
    }
    
    private void UpdateTimer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                isGameRunning = false;
                OnTimeUp?.Invoke();
            }
            
            UpdateTimerDisplay();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            timerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
            
            // Changement de couleur quand le temps est faible
            if (currentTime <= 60f)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }
    }
    
    // Méthodes publiques pour contrôler l'interface
    public void StartGame()
    {
        isGameRunning = true;
    }
    
    public void PauseGame()
    {
        isGameRunning = false;
    }
    
    public void ResumeGame()
    {
        isGameRunning = true;
    }
    
    public void UpdateProgress(float progress)
    {
        currentProgress = Mathf.Clamp01(progress);
        if (progressBar != null)
        {
            progressBar.value = currentProgress;
        }
    }
    
    public void SetScenarioName(string name)
    {
        scenarioName = name;
        if (scenarioNameText != null)
        {
            scenarioNameText.text = name;
        }
    }
    
    public void ResetTimer(float newTime = -1)
    {
        currentTime = newTime > 0 ? newTime : totalGameTime;
        UpdateTimerDisplay();
    }
    
    // Méthodes pour obtenir des informations
    public float GetRemainingTime()
    {
        return currentTime;
    }
    
    public float GetProgress()
    {
        return currentProgress;
    }
    
    public bool IsGameRunning()
    {
        return isGameRunning;
    }
}