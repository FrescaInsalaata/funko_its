using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // per fermare Play Mode nell'Editor
#endif
using System;

public class MenuSceneUI : MonoBehaviour
{
    // BACK → torna al MainMenu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // CONTROLLER → apre la ControllerScene
    public void GoToController()
    {
        SceneManager.LoadScene("ControllerScene");
    }

    // ESCI (opzionale, se vuoi aggiungerlo in futuro)
    public void QuitGame()
    {
        Debug.Log("[MenuSceneUI] QuitGame called");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // ferma Play Mode in Editor
#else
        Application.Quit(); // chiude nella build
        try
        {
            System.Environment.Exit(0); // forza la chiusura del processo se serve
        }
        catch (Exception e)
        {
            Debug.LogWarning("Environment.Exit failed: " + e);
        }
#endif
    }
}
