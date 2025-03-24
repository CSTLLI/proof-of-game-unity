using UnityEngine;
using Core;
using System.Collections;

public class WingIdentifier : MonoBehaviour
{
    [SerializeField] private string aileronId;
    [SerializeField] private string displayName;
    
    private ScenarioManager scenarioManager;
    
    [SerializeField] private bool isForMonaco;
    [SerializeField] private bool isAuthentic;
    
    void Start()
    {
        if (gameObject.tag != "Wing")
        {
            gameObject.tag = "Wing";
            Debug.LogWarning($"Tag 'Wing' ajouté à l'aileron {displayName}");
        }
        
        StartCoroutine(InitializeAfterDelay());
    }
    
    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
    
        Debug.Log($"InitializeAfterDelay pour {gameObject.name} - ID initial: {aileronId}");
    
        scenarioManager = FindFirstObjectByType<ScenarioManager>();
    
        if (scenarioManager != null && !string.IsNullOrEmpty(aileronId))
        {
            AileronData data = scenarioManager.GetAileronData(aileronId);
            if (data != null)
            {
                Debug.Log($"Données récupérées pour {aileronId}: {data.name}, isForMonaco={data.isForMonaco}, isAuthentic={data.isAuthentic}");
            
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
            Debug.LogWarning($"ScenarioManager non trouvé ou aileronId vide. gameObject.name={gameObject.name}, aileronId={aileronId}");
        }
    }
    
    public string GetAileronId()
    {
        Debug.Log($"GetAileronId appelé pour {gameObject.name}: ID = {aileronId}");
        return aileronId;
    }
    
    public string GetDisplayName()
    {
        return displayName;
    }
}