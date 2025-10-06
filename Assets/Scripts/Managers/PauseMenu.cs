using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // serve per riconoscere il controller

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Awake()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Controllo da tastiera (ESC)
        bool pausePressed = Input.GetKeyDown(KeyCode.Escape);

        // Controllo da controller (Start / Options)
        if (Gamepad.current != null)
        {
            if (Gamepad.current.startButton.wasPressedThisFrame)
                pausePressed = true;
        }

        // Se è stato premuto un tasto di pausa (ESC o Start)
        if (pausePressed)
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;  // Riavvia il tempo
        GameIsPaused = false;
    }

    void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;  // Ferma il tempo
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Reset tempo per sicurezza
        SceneManager.LoadScene("MainMenu"); // Cambia con il nome della tua scena menu
    }

    public void QuitGame()
    {
        Debug.Log("[PauseMenu] QuitGame called");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Ferma Play Mode in Editor
#else
        Application.Quit(); // Chiude il gioco in build
#endif
    }
}
