using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public class MainMenuUI : MonoBehaviour
{
    // START GAME → carica la GameScene
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // MENU → carica la MenuScene
    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // ESCI → chiude il gioco
    public void QuitGame()
    {
        Debug.Log("[MainMenuUI] QuitGame called");

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false; // ferma Play Mode in Editor
        #endif
    }
}
