using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Metodo per caricare la scena del gioco
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Usa il nome della tua scena di gioco
    }

    // Metodo per uscire dal gioco (funziona in build, non in editor)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game!"); // utile per debug in editor
    }
}
