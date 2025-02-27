using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TaskListUI : MonoBehaviour
{
    [SerializeField] private GameObject taskListPanel;
    [SerializeField] private GameObject taskItemPrefab;
    [SerializeField] private Transform taskContainer;
    [SerializeField] private float panelWidth = 300f;
    [SerializeField] private float taskItemHeight = 40f;
    [SerializeField] private Color completedTaskColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color pendingTaskColor = Color.white;
    
    private Dictionary<string, GameObject> taskItems = new Dictionary<string, GameObject>();
    private bool isPanelVisible = true;
    private ScenarioManager scenarioManager;
    
    void Start()
    {
        // Trouver le ScenarioManager
        scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager == null)
        {
            Debug.LogError("ScenarioManager introuvable!");
            return;
        }
        
        // Créer les éléments UI si nécessaire
        if (taskListPanel == null)
        {
            CreateTaskListPanel();
        }
        
        // Attendre que les tâches soient initialisées dans le ScenarioManager
        StartCoroutine(WaitForTasksAndInitialize());
    }
    
    private IEnumerator WaitForTasksAndInitialize()
    {
        // Attendre que le ScenarioManager ait fini de configurer les tâches
        yield return new WaitForSeconds(0.5f);
        
        // Récupérer et afficher les tâches
        var tasks = scenarioManager.GetTasks();
        if (tasks != null && tasks.Count > 0)
        {
            Initialize(tasks);
        }
        else
        {
            Debug.LogWarning("Aucune tâche n'a été trouvée dans le ScenarioManager!");
        }
    }
    
    public void Initialize(List<Task> tasks)
    {
        // Créer le panel si nécessaire
        if (taskListPanel == null)
        {
            CreateTaskListPanel();
        }
        
        // Effacer les tâches existantes
        ClearTaskItems();
        
        // Créer une entrée pour chaque tâche
        foreach (Task task in tasks)
        {
            AddTaskItem(task);
        }
    }
    
    private void CreateTaskListPanel()
    {
        // Créer un panel pour la liste des tâches
        taskListPanel = new GameObject("TaskListPanel");
        taskListPanel.transform.SetParent(transform, false);
        
        RectTransform panelRect = taskListPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.5f);
        panelRect.anchorMax = new Vector2(0, 0.5f);
        panelRect.pivot = new Vector2(0, 0.5f);
        panelRect.anchoredPosition = new Vector2(20, 0);
        panelRect.sizeDelta = new Vector2(panelWidth, 400);
        
        Image panelImage = taskListPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // Titre du panel
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(taskListPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(0, 50);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "TÂCHES À ACCOMPLIR";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontSize = 18;
        titleText.color = Color.white;
        
        // Conteneur pour les tâches
        GameObject containerObj = new GameObject("TaskContainer");
        containerObj.transform.SetParent(taskListPanel.transform, false);
        
        taskContainer = containerObj.transform;
        
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.offsetMin = new Vector2(10, 10);
        containerRect.offsetMax = new Vector2(-10, -60);
        
        // Créer un layout group pour organiser les tâches
        VerticalLayoutGroup layoutGroup = containerObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 5;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        
        // Ajouter un content size fitter
        ContentSizeFitter sizeFitter = containerObj.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Boutond de réduction/expansion
        GameObject toggleButton = new GameObject("ToggleButton");
        toggleButton.transform.SetParent(taskListPanel.transform, false);
        
        RectTransform toggleRect = toggleButton.AddComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(1, 1);
        toggleRect.anchorMax = new Vector2(1, 1);
        toggleRect.pivot = new Vector2(1, 1);
        toggleRect.anchoredPosition = new Vector2(-10, -10);
        toggleRect.sizeDelta = new Vector2(30, 30);
        
        Image toggleImage = toggleButton.AddComponent<Image>();
        toggleImage.color = new Color(0.3f, 0.3f, 0.3f);
        
        Button toggleBtn = toggleButton.AddComponent<Button>();
        toggleBtn.onClick.AddListener(TogglePanel);
        
        // Texte du bouton
        GameObject toggleTextObj = new GameObject("Text");
        toggleTextObj.transform.SetParent(toggleButton.transform, false);
        
        RectTransform toggleTextRect = toggleTextObj.AddComponent<RectTransform>();
        toggleTextRect.anchorMin = Vector2.zero;
        toggleTextRect.anchorMax = Vector2.one;
        toggleTextRect.offsetMin = Vector2.zero;
        toggleTextRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI toggleText = toggleTextObj.AddComponent<TextMeshProUGUI>();
        toggleText.text = "-";
        toggleText.alignment = TextAlignmentOptions.Center;
        toggleText.fontSize = 16;
        toggleText.color = Color.white;
    }
    
    private void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        
        // Mettre à jour l'affichage des tâches
        foreach (Transform child in taskContainer)
        {
            child.gameObject.SetActive(isPanelVisible);
        }
        
        // Mettre à jour le texte du bouton
        TextMeshProUGUI toggleText = taskListPanel.transform.Find("ToggleButton/Text").GetComponent<TextMeshProUGUI>();
        toggleText.text = isPanelVisible ? "-" : "+";
        
        // Ajuster la taille du panel
        RectTransform panelRect = taskListPanel.GetComponent<RectTransform>();
        if (isPanelVisible)
        {
            panelRect.sizeDelta = new Vector2(panelWidth, 400);
        }
        else
        {
            panelRect.sizeDelta = new Vector2(panelWidth, 50);
        }
    }
    
    private void ClearTaskItems()
    {
        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }
        taskItems.Clear();
    }
    
    private void AddTaskItem(Task task)
    {
        // Créer le prefab de l'item si nécessaire
        if (taskItemPrefab == null)
        {
            CreateTaskItemPrefab();
        }
        
        // Instancier l'item pour cette tâche
        GameObject taskItem = Instantiate(taskItemPrefab, taskContainer);
        
        // Configurer l'apparence de l'item
        RectTransform itemRect = taskItem.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, taskItemHeight);
        
        // Configurer le texte de la tâche
        TextMeshProUGUI taskText = taskItem.GetComponentInChildren<TextMeshProUGUI>();
        if (taskText != null)
        {
            taskText.text = task.description;
            taskText.color = task.isCompleted ? completedTaskColor : pendingTaskColor;
        }
        
        // Configurer l'état de complétion
        Toggle toggleComplete = taskItem.GetComponentInChildren<Toggle>();
        if (toggleComplete != null)
        {
            toggleComplete.isOn = task.isCompleted;
            toggleComplete.interactable = false; // Le joueur ne peut pas cocher manuellement
        }
        
        // Stocker pour référence
        taskItems[task.taskName] = taskItem;
    }
    
    private void CreateTaskItemPrefab()
    {
        // Créer le prefab pour les items de tâche
        taskItemPrefab = new GameObject("TaskItemPrefab");
        
        RectTransform itemRect = taskItemPrefab.AddComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, taskItemHeight);
        
        // Background
        Image bgImage = taskItemPrefab.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        
        // Checkbox
        GameObject checkboxObj = new GameObject("Checkbox");
        checkboxObj.transform.SetParent(taskItemPrefab.transform, false);
        
        RectTransform checkboxRect = checkboxObj.AddComponent<RectTransform>();
        checkboxRect.anchorMin = new Vector2(0, 0.5f);
        checkboxRect.anchorMax = new Vector2(0, 0.5f);
        checkboxRect.pivot = new Vector2(0, 0.5f);
        checkboxRect.anchoredPosition = new Vector2(10, 0);
        checkboxRect.sizeDelta = new Vector2(20, 20);
        
        Image checkboxImage = checkboxObj.AddComponent<Image>();
        checkboxImage.color = new Color(0.3f, 0.3f, 0.3f);
        
        Toggle checkbox = checkboxObj.AddComponent<Toggle>();
        checkbox.isOn = false;
        checkbox.interactable = false;
        
        // Checkmark
        GameObject checkmarkObj = new GameObject("Checkmark");
        checkmarkObj.transform.SetParent(checkboxObj.transform, false);
        
        RectTransform checkmarkRect = checkmarkObj.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = Vector2.zero;
        checkmarkRect.anchorMax = Vector2.one;
        checkmarkRect.offsetMin = new Vector2(4, 4);
        checkmarkRect.offsetMax = new Vector2(-4, -4);
        
        Image checkmarkImage = checkmarkObj.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.8f, 0.2f);
        
        checkbox.graphic = checkmarkImage;
        
        // Task Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(taskItemPrefab.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(40, 5);
        textRect.offsetMax = new Vector2(-5, -5);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;
        
        // Ne pas détruire lors de l'instanciation
        DontDestroyOnLoad(taskItemPrefab);
        taskItemPrefab.SetActive(false);
    }
    
    // Mettre à jour l'UI quand une tâche est complétée
    public void UpdateTaskStatus(string taskName, bool completed)
    {
        if (taskItems.TryGetValue(taskName, out GameObject taskItem))
        {
            Toggle toggleComplete = taskItem.GetComponentInChildren<Toggle>();
            if (toggleComplete != null)
            {
                toggleComplete.isOn = completed;
            }
            
            // Changer la couleur du texte
            TextMeshProUGUI taskText = taskItem.GetComponentInChildren<TextMeshProUGUI>();
            if (taskText != null)
            {
                taskText.color = completed ? completedTaskColor : pendingTaskColor;
            }
        }
    }
}