using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // usato solo in Editor
#endif
using System;

public class WinSceneUI : MonoBehaviour
{
    // Carica la scena di gioco
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Torna al menu principale
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Esci dal gioco (Editor: ferma Play Mode, Build: chiude l'eseguibile)
    public void QuitGame()
    {
        Debug.Log("[WinSceneUI] QuitGame called");

#if UNITY_EDITOR
        // Se siamo nell'Editor, ferma il Play Mode (utile per test veloci)
        EditorApplication.isPlaying = false;
#else
        // Questo chiude l'applicazione nella build standalone
        Application.Quit();

        // Fallback: forza l'uscita del processo se Application.Quit() non bastasse.
        // System.Environment.Exit(0) termina il processo .NET/Windows.
        // Lo eseguiamo subito dopo Application.Quit per essere sicuri che il processo termini.
        try
        {
            System.Environment.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[WinSceneUI] Environment.Exit failed: " + e);
        }
#endif
    }
}
