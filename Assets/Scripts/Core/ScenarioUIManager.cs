using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UI.Modal;
using UI.Feedback;
using Core;
using UI.UICore;

namespace Scenario
{
    public class ScenarioUIManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] public bool createUIOnStart = true;
        
        [Header("Modales")]
        [SerializeField] private GameObject introModalPrefab;
        [SerializeField] private GameObject completionModalPrefab;
        
        // Variables d'instance
        private GameObject introModalObj;
        private GameObject completionModalObj;
        private FeedbackUIController feedbackController;
        
        // Référence au ScenarioManager
        private ScenarioManager scenarioManager;
        
        // Singleton pattern
        public static ScenarioUIManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        void Start()
        {
            // Le ScenarioManager devrait appeler CreateAllUI
            if (createUIOnStart && scenarioManager == null)
            {
                CreateAllUI();
            }
        }
        
        // Méthode appelée par le ScenarioManager pour s'enregistrer
        public void RegisterScenarioManager(ScenarioManager manager)
        {
            scenarioManager = manager;
            Debug.Log("ScenarioManager enregistré auprès du ScenarioUIManager");
        }
        
        // Méthode unifiée pour créer toutes les UI
        public void CreateAllUI()
        {
            Debug.Log("Création de toutes les UI...");
            
            // Créer les systèmes UI dans l'ordre
            CreateFeedbackSystem();
            CreateIntroModal();
            CreateCompletionModal();
            CreateGameUI();
            
            Debug.Log("Création des UI terminée");
        }
        
        private void CreateGameUI()
        {
            // Vérifier si GameUIManager existe déjà
            if (FindFirstObjectByType<GameUIManager>() != null)
                return;
                
            GameObject uiCreator = new GameObject("UI_Creator");
            GameUICreator creator = uiCreator.AddComponent<GameUICreator>();
            
            Debug.Log("GameUI créée");
        }
        
        private void CreateIntroModal()
        {
            // Éviter de créer des doublons
            if (FindFirstObjectByType<GameIntroModal>() != null)
                return;
                
            if (introModalPrefab != null)
            {
                introModalObj = Instantiate(introModalPrefab);
                introModalObj.name = "IntroModal";
                DontDestroyOnLoad(introModalObj);
                return;
            }
            
            // Création programmatique
            introModalObj = new GameObject("IntroModal");
            DontDestroyOnLoad(introModalObj);
            
            Canvas canvas = introModalObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            introModalObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            introModalObj.AddComponent<GraphicRaycaster>();
            
            // Ajouter le GameIntroModal pour gérer la logique
            GameIntroModal introModalComp = introModalObj.AddComponent<GameIntroModal>();
            introModalComp.CreateModalUI();
            
            Debug.Log("IntroModal créée");
        }
        
        private void CreateCompletionModal()
        {
            // Éviter de créer des doublons
            if (FindFirstObjectByType<GameEndModal>() != null)
                return;
                
            if (completionModalPrefab != null)
            {
                completionModalObj = Instantiate(completionModalPrefab);
                completionModalObj.name = "CompletionModal";
                completionModalObj.SetActive(false);
                DontDestroyOnLoad(completionModalObj);
                return;
            }
            
            // Création programmatique
            completionModalObj = new GameObject("CompletionModal");
            DontDestroyOnLoad(completionModalObj);
            
            Canvas canvas = completionModalObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            completionModalObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            completionModalObj.AddComponent<GraphicRaycaster>();
            
            // Ajouter le GameEndModal
            completionModalObj.AddComponent<GameEndModal>();
            completionModalObj.SetActive(false);
            
            Debug.Log("CompletionModal créée");
        }
        
        private void CreateFeedbackSystem()
        {
            // Éviter de créer des doublons
            if (GameObject.Find("FeedbackCanvas") != null)
                return;
                
            GameObject feedbackCanvas = new GameObject("FeedbackCanvas");
            DontDestroyOnLoad(feedbackCanvas);
            
            Canvas canvas = feedbackCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            
            feedbackCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            feedbackCanvas.AddComponent<GraphicRaycaster>();
            
            GameObject tempMsgPanel = new GameObject("TempMessagePanel");
            tempMsgPanel.transform.SetParent(feedbackCanvas.transform, false);
            
            RectTransform tempRect = tempMsgPanel.AddComponent<RectTransform>();
            tempRect.anchorMin = new Vector2(0.5f, 0);
            tempRect.anchorMax = new Vector2(0.5f, 0);
            tempRect.pivot = new Vector2(0.5f, 0);
            tempRect.anchoredPosition = new Vector2(0, 80);
            tempRect.sizeDelta = new Vector2(500, 60);
            
            Image tempImage = tempMsgPanel.AddComponent<Image>();
            tempImage.color = new Color(0, 0, 0, 0.7f);
            
            GameObject tempTextObj = new GameObject("TempMessageText");
            tempTextObj.transform.SetParent(tempMsgPanel.transform, false);
            
            RectTransform tempTextRect = tempTextObj.AddComponent<RectTransform>();
            tempTextRect.anchorMin = Vector2.zero;
            tempTextRect.anchorMax = Vector2.one;
            tempTextRect.offsetMin = new Vector2(10, 5);
            tempTextRect.offsetMax = new Vector2(-10, -5);
            
            TextMeshProUGUI tempText = tempTextObj.AddComponent<TextMeshProUGUI>();
            tempText.alignment = TextAlignmentOptions.Center;
            tempText.fontSize = 16;
            tempText.color = Color.white;
            
            feedbackController = feedbackCanvas.AddComponent<FeedbackUIController>();
            feedbackController.Initialize(null, tempMsgPanel, tempText);
            
            tempMsgPanel.SetActive(false);
            
            Debug.Log("FeedbackSystem créé");
        }
        
        public void ShowIntroModal()
        {
            GameIntroModal introModal = FindFirstObjectByType<GameIntroModal>();
            if (introModal != null)
            {
                introModal.Show();
            }
            else
            {
                Debug.LogError("IntroModal introuvable");
            }
        }
        
        public void ShowCompletionModal(bool success, int aileronsFound, int requiredAilerons, float timeElapsed, float riskLevel)
        {
            GameEndModal endModal = FindFirstObjectByType<GameEndModal>();
            if (endModal != null)
            {
                endModal.ShowResults(success, aileronsFound, requiredAilerons, timeElapsed, riskLevel);
            }
            else
            {
                Debug.LogError("EndModal introuvable");
            }
        }
        
        public void ShowTemporaryMessage(string message, float duration = 3f)
        {
            if (feedbackController == null)
                feedbackController = FindFirstObjectByType<FeedbackUIController>();
                
            if (feedbackController != null)
                feedbackController.ShowTemporaryMessage(message, duration);
            else
                Debug.LogError("FeedbackController introuvable");
        }
        
        public void DebugUIStatus()
        {
            GameIntroModal introModal = FindFirstObjectByType<GameIntroModal>();
            GameEndModal endModal = FindFirstObjectByType<GameEndModal>();
            GameUIManager gameUIManager = FindFirstObjectByType<GameUIManager>();
            FeedbackUIController feedback = FindFirstObjectByType<FeedbackUIController>();
            
            Debug.Log("Status des UI: " +
                     "IntroModal=" + (introModal != null ? "OK" : "NULL") + ", " +
                     "EndModal=" + (endModal != null ? "OK" : "NULL") + ", " +
                     "GameUIManager=" + (gameUIManager != null ? "OK" : "NULL") + ", " +
                     "FeedbackController=" + (feedback != null ? "OK" : "NULL"));
        }
    }
}