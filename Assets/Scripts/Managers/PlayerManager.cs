using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerPrefab;      // Prefab del player
    public Transform[] spawnPoints;      // Punti di spawn (massimo 4)

    private int playerCount = 0;         // Conta quanti player sono stati spawnati

    void Start()
    {
        // Controlla se ci sono già player nella scena
        PlayerBehaviour[] existingPlayers = FindObjectsByType<PlayerBehaviour>(FindObjectsSortMode.None);
        Debug.Log("Player esistenti trovati: " + existingPlayers.Length);
        foreach (var player in existingPlayers)
        {
            if (playerCount < spawnPoints.Length)
            {
                player.playerID = playerCount;

                var health = player.GetComponent<Health>();
                if (health != null)
                    health.playerID = playerCount;

                if (UIManager.Instance != null)
                    UIManager.Instance.playerUIs[playerCount].gameObject.SetActive(true);

                playerCount++;
            }
            else
            {
                Debug.LogWarning("Numero massimo di player raggiunto, player aggiuntivo ignorato!");
            }
        }
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayer();
        }
    }

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
        var pb = newPlayer.GetComponent<PlayerBehaviour>();
        pb.playerID = playerCount;

        var health = newPlayer.GetComponent<Health>();
        health.playerID = playerCount;

        Debug.Log("Player " + playerCount + " spawnato.");

        // Attiva la UI corrispondente al player
        if (UIManager.Instance != null)
        {
            UIManager.Instance.playerUIs[playerCount].gameObject.SetActive(true);
        }

        playerCount++;
    }
}
