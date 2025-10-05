using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public int playerCount = 0;

    public void SpawnPlayerUI(GameObject newPlayer)
    {
        var pb = newPlayer.GetComponent<PlayerBehaviour>();
        pb.playerID = playerCount;

        var health = newPlayer.GetComponent<Health>();
        health.playerID = playerCount;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.playerUIs[playerCount].gameObject.SetActive(true);
        }
        Debug.Log("Player UI " + playerCount + " spawnata.");
        playerCount++;
    }
}
