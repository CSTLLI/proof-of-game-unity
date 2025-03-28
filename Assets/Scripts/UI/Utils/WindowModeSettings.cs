using UnityEngine;

public class WindowModeSettings : MonoBehaviour
{
    [SerializeField] private bool startInWindowedMode = true;
    [SerializeField] private int windowWidth = 1280;
    [SerializeField] private int windowHeight = 720;
    
    void Awake()
    {
        // Démarrer en mode fenêtré
        if (startInWindowedMode)
        {
            Screen.fullScreen = false;
            
            Screen.SetResolution(windowWidth, windowHeight, false);
            
            #if UNITY_STANDALONE_WIN
                        SetWindowResizable();
            #endif
                    }
                }
                
            #if UNITY_STANDALONE_WIN
                // Import des fonctions natives de Windows pour rendre la fenêtre redimensionnable
                [System.Runtime.InteropServices.DllImport("user32.dll")]
                private static extern System.IntPtr GetActiveWindow();
                
                [System.Runtime.InteropServices.DllImport("user32.dll")]
                private static extern int SetWindowLong(System.IntPtr hWnd, int nIndex, int dwNewLong);
                
                [System.Runtime.InteropServices.DllImport("user32.dll")]
                private static extern int GetWindowLong(System.IntPtr hWnd, int nIndex);
                
                private void SetWindowResizable()
                {
                    var hwnd = GetActiveWindow();
                    int style = GetWindowLong(hwnd, -16); // GWL_STYLE
                    
                    // Ajouter le style pour rendre la fenêtre redimensionnable et maximisable
                    // 0x00040000 = WS_SIZEBOX, 0x00010000 = WS_MAXIMIZEBOX
                    SetWindowLong(hwnd, -16, style | 0x00040000 | 0x00010000);
                }
            #endif
}