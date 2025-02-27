using UnityEngine;

public class SimpleCrosshair : MonoBehaviour
{
    public Texture2D crosshairTexture;
    public int crosshairSize = 20;
    public Color crosshairColor = Color.white;

    void OnGUI()
    {
        if (crosshairTexture != null)
        {
            // Calculer la position au centre de l'écran
            float posX = (Screen.width / 2) - (crosshairSize / 2);
            float posY = (Screen.height / 2) - (crosshairSize / 2);
            
            // Sauvegarder la couleur actuelle du GUI
            Color originalColor = GUI.color;
            
            // Appliquer notre couleur
            GUI.color = crosshairColor;
            
            // Dessiner le curseur
            GUI.DrawTexture(new Rect(posX, posY, crosshairSize, crosshairSize), crosshairTexture);
            
            // Restaurer la couleur d'origine
            GUI.color = originalColor;
        }
        else
        {
            // Si pas de texture, dessiner un simple "+"
            GUIStyle crosshairStyle = new GUIStyle();
            crosshairStyle.normal.textColor = crosshairColor;
            crosshairStyle.fontSize = 24;
            crosshairStyle.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 - 10, 20, 20), "•", crosshairStyle);
        }
    }
}