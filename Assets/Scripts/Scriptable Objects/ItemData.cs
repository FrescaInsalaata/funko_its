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
    public float duration; // Duration of the consumable effect
    public float multSpeedBoost; 
    /*public float delay;
    public float delayTime;
    public float durationTime;
    public float throwForce;*/
    public void Update()
    {
        
    }
    public virtual void UseItem(Transform throwPoint)
    {
        /*if (itemType == ItemType.Throwable)
        {
            UseThrowable(throwPoint);
        }
        else */
        if (itemType == ItemType.Consumable)
        {
            // Assuming the player GameObject has a tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            UseConsumable(player);
        }
    }
    /*
    public virtual void UseThrowable(Transform throwPoint)
    {
        if (itemType == ItemType.Throwable && itemPrefab != null && throwPoint != null)
        {
            GameObject item = Instantiate(itemPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
            }
        }
    }*/
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
