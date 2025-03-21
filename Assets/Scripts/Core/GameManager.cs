using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string gameSceneName = "QualityControl";
        
        private bool isGameStarted = false;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartGame()
        {
            if (!isGameStarted)
            {
                isGameStarted = true;
                SceneManager.LoadScene(gameSceneName);
            }
        }
        
        public void ReturnToMainMenu()
        {
            isGameStarted = false;
            SceneManager.LoadScene(mainMenuSceneName);
        }
        
        public void QuitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }
    }
}