using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerUI[] playerUIs; // array di 4 pannelli UI

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var ui in playerUIs)
            ui.gameObject.SetActive(false);
    }

    public void updateHealth(int playerID, int health)
    {
        if (playerID < 0)
            return;
        playerUIs[playerID].setHealth(health);
    }

    public void updateAmmo(int playerID, int ammo)
    {
        if (playerID < 0)
            return;
        playerUIs[playerID].setAmmo(ammo);
    }
}
