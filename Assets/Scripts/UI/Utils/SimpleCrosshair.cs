using UnityEngine;

namespace UI.Utils
{
    public class SimpleCrosshair : MonoBehaviour
    {
        public Texture2D crosshairTexture;
        public int crosshairSize = 20;
        public Color crosshairColor = Color.white;
        
        void OnGUI()
        {
            if (crosshairTexture != null)
            {
                float posX = (Screen.width / 2) - (crosshairSize / 2);
                float posY = (Screen.height / 2) - (crosshairSize / 2);
                
                Color originalColor = GUI.color;
                GUI.color = crosshairColor;
                
                GUI.DrawTexture(new Rect(posX, posY, crosshairSize, crosshairSize), crosshairTexture);
                
                GUI.color = originalColor;
            }
            else
            {
                GUIStyle crosshairStyle = new GUIStyle();
                crosshairStyle.normal.textColor = crosshairColor;
                crosshairStyle.fontSize = 24;
                crosshairStyle.alignment = TextAnchor.MiddleCenter;
                
                GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 - 10, 20, 20), "•", crosshairStyle);
            }
        }
    }
}