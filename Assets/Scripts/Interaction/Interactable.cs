using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactionText = "interagir";
    public KeyCode interactionKey = KeyCode.E;
    
    public abstract void Interact();
    
    public virtual void OnFocus()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.yellow;
        }
        Debug.Log($"Regardant: {gameObject.name}");
    }
    
    public virtual void OnLoseFocus()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.white;
        }
    }
}