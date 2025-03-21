using UnityEngine;
using Core;
using System.Collections;

public class WingIdentifier : MonoBehaviour
{
    [SerializeField] private string aileronId; // ID correspondant à la clé dans le dictionnaire du ScenarioManager
    [SerializeField] private string displayName; // Nom à afficher à l'utilisateur
    
    private ScenarioManager scenarioManager;
    
    // Pour déboguer : montre si cet aileron est pour Monaco
    [SerializeField] private bool isForMonaco;
    [SerializeField] private bool isAuthentic;
    
    void Start()
    {
        // S'assurer que cet objet a le tag "wing"
        if (gameObject.tag != "Wing")
        {
            gameObject.tag = "Wing";
            Debug.LogWarning($"Tag 'Wing' ajouté à l'aileron {displayName}");
        }
        
        // Lancer une coroutine pour attendre l'initialisation du ScenarioManager
        StartCoroutine(InitializeAfterDelay());
    }
    
    private IEnumerator InitializeAfterDelay()
    {
        // Attendre quelques frames pour s'assurer que le ScenarioManager est initialisé
        yield return new WaitForSeconds(1.0f);
        
        // Trouver le ScenarioManager
        scenarioManager = FindObjectOfType<ScenarioManager>();
        
        // Optionnel : vérifier que l'ID existe dans le ScenarioManager
        if (scenarioManager != null && !string.IsNullOrEmpty(aileronId))
        {
            AileronData data = scenarioManager.GetAileronData(aileronId);
            if (data != null)
            {
                isForMonaco = data.isForMonaco;
                isAuthentic = data.isAuthentic;
                
                if (string.IsNullOrEmpty(displayName))
                    displayName = data.name;
                
                Debug.Log($"Aileron {aileronId} initialisé avec succès: {displayName}");
            }
            else
            {
                Debug.LogError($"L'ID d'aileron '{aileronId}' n'existe pas dans le ScenarioManager");
            }
        }
        else
        {
            Debug.LogWarning("ScenarioManager non trouvé ou aileronId vide");
        }
    }
    
    public string GetAileronId()
    {
        return aileronId;
    }
    
    public string GetDisplayName()
    {
        return displayName;
    }
}