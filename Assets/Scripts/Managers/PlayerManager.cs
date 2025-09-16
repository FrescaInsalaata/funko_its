using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerPrefab;      // Prefab del player
    public Transform[] spawnPoints;      // Punti di spawn (massimo 4)

    private int playerCount = 0;         // Conta quanti player sono stati spawnati

    // Funzione pubblica per spawnare un nuovo player
    public void SpawnPlayer()
    {
        if (playerCount >= spawnPoints.Length)
        {
            Debug.LogWarning("Numero massimo di player raggiunto!");
            return;
        }

        // Istanzia il player
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoints[playerCount].position, spawnPoints[playerCount].rotation);

        // Assegna un ID univoco
        newPlayer.GetComponent<PlayerBehaviour>().playerID = playerCount;
        newPlayer.GetComponent<Health>().playerID = playerCount;

        Debug.Log("Player " + playerCount + " spawnato.");

        playerCount++;
    }
}
