using UnityEngine;
using TMPro;
using System.Collections;
using Core;

namespace Scenario.Task
{
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
            
            UpdateTaskText("Trouvez et validez les 2 ailerons pour Monaco", false);
        }
        
        void Start()
        {
            if (titleText == null)
                titleText = transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
                
            if (descriptionText == null)
                descriptionText = transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
                
            if (scenarioManager == null)
                scenarioManager = FindObjectOfType<ScenarioManager>();
                
            if (titleText != null && descriptionText != null)
                UpdateTaskText("Trouvez et validez les 2 ailerons pour Monaco", false);
        }
        
        private void UpdateTaskText(string text, bool requiresBlockchain)
        {
            if (titleText == null || descriptionText == null) return;
            
            titleText.text = "Tâche:";
            descriptionText.text = text;
            descriptionText.color = requiresBlockchain ? blockchainTaskColor : standardTaskColor;
        }
        
        public void NotifyTaskCompleted(string taskName)
        {
            if (lastTaskName == taskName)
            {
                lastTaskName = "";
                UpdateTaskText("Trouvez et validez les 2 ailerons pour Monaco", false);
            }
        }
    }
}