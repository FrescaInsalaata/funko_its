using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public class ControllerSceneUI : MonoBehaviour
{
    // BACK → torna al MainMenu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // ESCI (opzionale se vuoi aggiungerlo in futuro)
    public void QuitGame()
    {
        Debug.Log("[ControllerSceneUI] QuitGame called");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
        try
        {
            System.Environment.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Environment.Exit failed: " + e);
        }
#endif
    }
}
