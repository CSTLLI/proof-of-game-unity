using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace UI.Modal
{
    public class GameEndModal : MonoBehaviour
    {
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI resultsText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI riskText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        
        void Start()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClicked);
                
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
                
            if (modalPanel != null)
                modalPanel.SetActive(false);
        }
        
        public void ShowResults(bool success, int aileronsFound, int requiredAilerons, float timeElapsed, float riskLevel)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            string timeString = $"{minutes:00}:{seconds:00}";
            
            if (titleText != null)
            {
                titleText.text = success ? "MISSION ACCOMPLIE" : "TEMPS ÉCOULÉ";
                titleText.color = success ? Color.green : Color.red;
            }
            
            if (resultsText != null)
            {
                if (success)
                    resultsText.text = $"Vous avez validé les {requiredAilerons} ailerons pour Monaco!";
                else
                    resultsText.text = $"Vous avez trouvé seulement {aileronsFound}/{requiredAilerons} ailerons pour Monaco.";
            }
            
            if (timeText != null)
                timeText.text = $"Temps: {timeString}";
                
            if (riskText != null)
            {
                riskText.text = $"Niveau de risque: {riskLevel:0}%";
                
                if (riskLevel < 30f)
                    riskText.color = Color.green;
                else if (riskLevel < 60f)
                    riskText.color = Color.yellow;
                else
                    riskText.color = Color.red;
            }
            
            ScenarioManager scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.DisablePlayerControls(true);
            }
    
            // Activer la modale
            gameObject.SetActive(true);
        }
        
        void OnRestartButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        
        void OnQuitButtonClicked()
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}