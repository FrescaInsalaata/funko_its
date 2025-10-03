using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;

    private int ammo;

    public void setAmmo(int value)
    {
        ammo = value;
        ammoText.text = ammo.ToString();
    }

    public void setHealth(int value)
    {
        healthText.text = value.ToString();
    }
}
