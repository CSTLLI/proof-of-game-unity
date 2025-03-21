using UnityEngine;

namespace Interaction
{
    public class PlayerInteractionController : MonoBehaviour
    {
        public float interactionDistance = 3f;
        public LayerMask interactionMask = -1;
        
        private bool showInteractionText = false;
        private string interactionMessage = "";
        
        private Camera playerCamera;
        private Interactable currentInteractable;
        
        void Start()
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("Aucune caméra trouvée sur le joueur.");
                enabled = false;
            }
        }
        
        void Update()
        {
            CheckForInteractable();
            HandleInteractionInput();
        }
        
        void CheckForInteractable()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);
            
            if (Physics.Raycast(ray, out hit, interactionDistance, interactionMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                
                if (interactable != null)
                {
                    if (currentInteractable != interactable)
                    {
                        if (currentInteractable != null)
                            currentInteractable.OnLoseFocus();
                            
                        currentInteractable = interactable;
                        currentInteractable.OnFocus();
                        
                        showInteractionText = true;
                        interactionMessage = $"Appuyez sur [{currentInteractable.interactionKey}] pour {currentInteractable.interactionText}";
                    }
                }
                else
                {
                    ClearInteractable();
                }
            }
            else
            {
                ClearInteractable();
            }
        }
        
        void ClearInteractable()
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
                showInteractionText = false;
            }
        }
        
        void HandleInteractionInput()
        {
            if (currentInteractable != null && Input.GetKeyDown(currentInteractable.interactionKey))
            {
                Debug.Log($"Interaction avec {currentInteractable.gameObject.name}");
                currentInteractable.Interact();
            }
        }
        
        void OnGUI()
        {
            if (showInteractionText)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 14;
                style.alignment = TextAnchor.LowerCenter;
                style.padding = new RectOffset(10, 10, 10, 10);
                style.wordWrap = true;
                
                float boxWidth = 300;
                float boxHeight = 30;
                
                Rect textPosition = new Rect(
                    (Screen.width - boxWidth) / 2, 
                    Screen.height - boxHeight - 20, 
                    boxWidth, boxHeight);
                
                Texture2D backgroundTexture = new Texture2D(1, 1);
                backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
                backgroundTexture.Apply();
                
                GUI.DrawTexture(textPosition, backgroundTexture);
                GUI.Label(textPosition, interactionMessage, style);
            }
        }
    }
}