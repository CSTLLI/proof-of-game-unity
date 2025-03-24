using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace UI.Modal
{
    public class GameIntroModal : MonoBehaviour
    {
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button startButton;
        
        private ScenarioManager scenarioManager;

        void Start()
        {
            if (titleText != null)
                titleText.text = "Contrôle Qualité - Grand Prix de Monaco";

            if (descriptionText != null)
                descriptionText.text = "Le Grand Prix de Monaco approche et la Scuderia Ferrari a besoin de vous!\n\n" +
                                       "MISSION: Parmi les 6 ailerons présents dans l'atelier, identifiez et validez les 2 ailerons spécifiquement conçus pour Monaco.\n\n" +
                                       "ATTENTION: Certains ailerons sont des contrefaçons ou conçus pour d'autres circuits.";

            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
            
            scenarioManager = FindFirstObjectByType<ScenarioManager>();
        }

        public void CreateModalUI()
        {
            // Créer le panel principal
            modalPanel = new GameObject("ModalPanel");
            modalPanel.transform.SetParent(transform, false);

            // Ajouter Canvas
            Canvas canvas = modalPanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            modalPanel.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            modalPanel.AddComponent<GraphicRaycaster>();

            // Créer le fond
            GameObject background = new GameObject("Background");
            background.transform.SetParent(modalPanel.transform, false);
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);

            // Créer le contenu
            GameObject content = new GameObject("Content");
            content.transform.SetParent(modalPanel.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(600, 400);
            contentRect.anchoredPosition = Vector2.zero;
            Image contentImage = content.AddComponent<Image>();
            contentImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Créer le titre
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(content.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 60);
            titleRect.anchoredPosition = new Vector2(0, -30);
            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Contrôle Qualité - Grand Prix de Monaco";
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;

            // Créer la description
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(content.transform, false);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0);
            descRect.anchorMax = new Vector2(1, 1);
            descRect.sizeDelta = new Vector2(-40, -120);
            descRect.anchoredPosition = new Vector2(0, -20);
            descriptionText = descObj.AddComponent<TextMeshProUGUI>();
            descriptionText.text = "Le Grand Prix de Monaco approche et la Scuderia Ferrari a besoin de vous!\n\n" +
                                   "MISSION: Parmi les 6 ailerons présents dans l'atelier, identifiez et validez les 2 ailerons spécifiquement conçus pour Monaco.\n\n" +
                                   "ATTENTION: Certains ailerons sont des contrefaçons ou conçus pour d'autres circuits.";
            descriptionText.fontSize = 18;
            descriptionText.alignment = TextAlignmentOptions.Center;
            descriptionText.color = Color.white;

            // Créer le bouton de démarrage
            GameObject buttonObj = new GameObject("StartButton");
            buttonObj.transform.SetParent(content.transform, false);
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0);
            buttonRect.anchorMax = new Vector2(0.5f, 0);
            buttonRect.sizeDelta = new Vector2(200, 50);
            buttonRect.anchoredPosition = new Vector2(0, 40);
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f);
            startButton = buttonObj.AddComponent<Button>();
            startButton.targetGraphic = buttonImage;
            startButton.interactable = true;

            // Texte du bouton
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "COMMENCER LA MISSION";
            buttonText.fontSize = 16;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;

            startButton.onClick.AddListener(OnStartButtonClicked);

            Show();
        }

        void OnStartButtonClicked()
        {
            Debug.Log("Bouton de démarrage cliqué!");
            
            if (modalPanel != null)
                modalPanel.SetActive(false);
            
            if (scenarioManager == null)
                scenarioManager = FindFirstObjectByType<ScenarioManager>();
                
            if (scenarioManager != null)
            {
                Debug.Log("Démarrage du scénario...");
                scenarioManager.StartScenario();
            }
            else
            {
                Debug.LogError("ScenarioManager introuvable!");
            }
        }

        public void Show()
        {
            if (modalPanel != null)
                modalPanel.SetActive(true);
        }
    }
}