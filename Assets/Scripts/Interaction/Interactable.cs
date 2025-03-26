using UnityEngine;
using System.Collections.Generic;

namespace Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        public string interactionText = "interagir";
        public KeyCode interactionKey = KeyCode.E;
        
        [Header("Mise en évidence")]
        [Tooltip("Si activé, applique la mise en évidence aux objets enfants")]
        public bool highlightChildren = false;
        [Tooltip("Référence optionnelle vers un objet spécifique à mettre en évidence")]
        public GameObject highlightTarget;
        [Tooltip("Intensité de la spécularité lorsque l'objet est ciblé")]
        public float highlightSpecularIntensity = 1.0f;
        [Tooltip("Couleur de la spécularité lorsque l'objet est ciblé")]
        public Color highlightSpecularColor = Color.yellow;
        
        private List<Material> materialsList = new List<Material>();
        private List<float> originalSpecularValues = new List<float>();
        private List<Color> originalSpecularColors = new List<Color>();
        
        protected virtual void Start()
        {
            // Cache les références nécessaires
        }
        
        public abstract void Interact();
        
        public virtual void OnFocus()
        {
            ApplySpecularHighlight();
            Debug.Log($"Regardant: {gameObject.name}");
        }
        
        public virtual void OnLoseFocus()
        {
            RemoveSpecularHighlight();
        }
        
        protected void ApplySpecularHighlight()
        {
            // Réinitialiser les listes
            materialsList.Clear();
            originalSpecularValues.Clear();
            originalSpecularColors.Clear();
            
            // Déterminer quels renderers cibler
            Renderer[] targetRenderers;
            if (highlightTarget != null)
            {
                targetRenderers = highlightTarget.GetComponentsInChildren<Renderer>();
            }
            else if (highlightChildren)
            {
                targetRenderers = GetComponentsInChildren<Renderer>();
            }
            else
            {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                    targetRenderers = new Renderer[] { renderer };
                else
                    targetRenderers = new Renderer[0];
            }
            
            // Ajouter l'effet spéculaire à chaque matériau
            foreach (Renderer renderer in targetRenderers)
            {
                if (renderer == null) continue;
                
                // Traiter chaque matériau du renderer
                foreach (Material mat in renderer.materials)
                {
                    materialsList.Add(mat);
                    
                    // Sauvegarder les valeurs originales
                    float originalSpecular = 0f;
                    if (mat.HasProperty("_Glossiness")) 
                        originalSpecular = mat.GetFloat("_Glossiness");
                    else if (mat.HasProperty("_Smoothness"))
                        originalSpecular = mat.GetFloat("_Smoothness");
                    
                    Color originalColor = Color.white;
                    if (mat.HasProperty("_SpecColor"))
                        originalColor = mat.GetColor("_SpecColor");
                    else if (mat.HasProperty("_SpecularColor"))
                        originalColor = mat.GetColor("_SpecularColor");
                    
                    originalSpecularValues.Add(originalSpecular);
                    originalSpecularColors.Add(originalColor);
                    
                    // Appliquer la mise en évidence spéculaire
                    if (mat.HasProperty("_Glossiness"))
                        mat.SetFloat("_Glossiness", highlightSpecularIntensity);
                    if (mat.HasProperty("_Smoothness"))
                        mat.SetFloat("_Smoothness", highlightSpecularIntensity);
                    
                    if (mat.HasProperty("_SpecColor"))
                        mat.SetColor("_SpecColor", highlightSpecularColor);
                    if (mat.HasProperty("_SpecularColor"))
                        mat.SetColor("_SpecularColor", highlightSpecularColor);
                    
                    // Si le shader a besoin d'une carte spéculaire
                    if (mat.HasProperty("_SpecularMap") && mat.GetTexture("_SpecularMap") == null)
                    {
                        // Créer une texture temporaire pour la spécularité
                        Texture2D specMap = new Texture2D(2, 2);
                        Color[] colors = new Color[4] { 
                            highlightSpecularColor, 
                            highlightSpecularColor, 
                            highlightSpecularColor, 
                            highlightSpecularColor 
                        };
                        specMap.SetPixels(colors);
                        specMap.Apply();
                        
                        mat.SetTexture("_SpecularMap", specMap);
                    }
                }
            }
        }
        
        protected void RemoveSpecularHighlight()
        {
            // Restaurer les valeurs originales
            for (int i = 0; i < materialsList.Count; i++)
            {
                Material mat = materialsList[i];
                if (mat == null) continue;
                
                if (i < originalSpecularValues.Count)
                {
                    if (mat.HasProperty("_Glossiness"))
                        mat.SetFloat("_Glossiness", originalSpecularValues[i]);
                    if (mat.HasProperty("_Smoothness"))
                        mat.SetFloat("_Smoothness", originalSpecularValues[i]);
                }
                
                if (i < originalSpecularColors.Count)
                {
                    if (mat.HasProperty("_SpecColor"))
                        mat.SetColor("_SpecColor", originalSpecularColors[i]);
                    if (mat.HasProperty("_SpecularColor"))
                        mat.SetColor("_SpecularColor", originalSpecularColors[i]);
                }
            }
            
            // Vider les listes
            materialsList.Clear();
            originalSpecularValues.Clear();
            originalSpecularColors.Clear();
        }
    }
}