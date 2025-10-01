using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // per fermare il Play Mode nell'Editor
#endif
using System;

public class GameOverUI : MonoBehaviour
{
    // START → carica la GameScene
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // BACK TO MENU → carica il MainMenu
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // ESCI → chiude il gioco
    public void QuitGame()
    {
        Debug.Log("[GameOverUI] QuitGame called");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // ferma Play nell’Editor
#else
        Application.Quit(); // chiude nella build
        try
        {
            System.Environment.Exit(0); // forza chiusura se necessario
        }
        catch (Exception e)
        {
            Debug.LogWarning("Environment.Exit failed: " + e);
        }
#endif
    }
}
