using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UI.Feedback
{
    public class FeedbackUIController : MonoBehaviour
    {
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private GameObject tempMessagePanel;
        [SerializeField] private TextMeshProUGUI tempMessageText;
        
        private Coroutine activeMessageCoroutine;
        
        public void Initialize(GameObject feedbackPanel, GameObject tempMessagePanel, TextMeshProUGUI tempMessageText)
        {
            this.feedbackPanel = feedbackPanel;
            this.tempMessagePanel = tempMessagePanel;
            this.tempMessageText = tempMessageText;
        }
        
        public void ShowTemporaryMessage(string message, float duration = 3f)
        {
            if (tempMessagePanel == null || tempMessageText == null) return;
            if (activeMessageCoroutine != null)
                StopCoroutine(activeMessageCoroutine);
                
            activeMessageCoroutine = StartCoroutine(ShowMessageCoroutine(message, duration));
        }
        
        public void ShowFeedback(string title, string message, bool isSuccess)
        {
            if (feedbackPanel == null) return;
            
            var titleText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = title;
                titleText.color = isSuccess ? Color.green : Color.red;
            }
            
            feedbackPanel.SetActive(true);
        }
        
        private IEnumerator ShowMessageCoroutine(string message, float duration)
        {
            tempMessageText.text = message;
            tempMessagePanel.SetActive(true);
            
            yield return new WaitForSeconds(duration);
            
            tempMessagePanel.SetActive(false);
            activeMessageCoroutine = null;
        }
    }
}