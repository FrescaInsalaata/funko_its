using Unity.VisualScripting;
using UnityEngine;
public enum ItemType { Throwable, Consumable }

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public GameObject itemPrefab;

    [Header("Item Stats")]
    public float duration;
    public float multSpeedBoost = 2f; 
    public virtual void UseItem(GameObject user)
    {
        if (itemType == ItemType.Consumable)
        {
            UseConsumable(user);
        }
    }
    public virtual void UseConsumable(GameObject player)
    {
        if (itemType == ItemType.Consumable && player != null)
        {
            /*VIBECHECK! (Vibe Check!, “Literally just the character saying Vibe Check or smth”),
            applies a wave of fun for teammates in a short area, letting them move faster*/
            if (itemName == "VibeCheck")
            {
                Debug.Log("Using VibeCheck on " + player.name);
                player.GetComponent<PlayerBehaviour>().ApplyVibeCheck(multSpeedBoost, duration);
                Destroy(this);
            }
        }
    }
}
