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
    public virtual void UseItem(Transform throwPoint)
    {
        if (itemType == ItemType.Consumable)
        {
            // Assuming the player GameObject has a tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            UseConsumable(player);
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
                player.GetComponent<PlayerBehaviour>().ApplyVibeCheck(multSpeedBoost, duration);
                Destroy(this);
            }
        }
    }
}
