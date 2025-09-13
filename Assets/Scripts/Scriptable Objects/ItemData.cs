using UnityEngine;
public enum ItemType { Throwable, Consumable }

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public GameObject itemPrefab;

    [Header("Item Stats")]
    public float uses;
    public float throwForce; // For throwable items
    public float healAmount; // For consumable items
    public float healRate;
    public float duration; // Duration of the consumable effect
    public float delay;
    public float delayTime;
    public float durationTime;

    public virtual void UseItem(Transform throwPoint)
    {
        if (itemType == ItemType.Throwable)
        {
            UseThrowable(throwPoint);
        }
        else if (itemType == ItemType.Consumable)
        {
            // Assuming the player GameObject has a tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            UseConsumable(player);
        }
    }
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
    }
    public virtual void UseConsumable(GameObject player)
    {
        if (itemType == ItemType.Consumable && player != null)
        {
            //M1LLY (Cassette Launcher), throws explosive cassettes that detonate after a short delay. Area-of-effect but limited ammo.
            if (itemName == "M1LLY")
            {
                GameObject item = Instantiate(itemPrefab, player.transform.position + player.transform.forward + new Vector3(0, 1, 0), player.transform.rotation);
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(player.transform.forward * throwForce, ForceMode.VelocityChange);
                }
                if (uses <= 0)
                {
                    Debug.Log("Out of uses for " + itemName);
                    return;
                }
                else uses--;
                return;
            }
            /*VIBECHECK! (Vibe Check!, “Literally just the character saying Vibe Check or smth”),
            applies a wave of fun for teammates in a short area, healing them for a short duration and speeding them up.*/
            if (itemName == "VibeCheck")
            {
                //Apply effect to all players in a range
                Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 5f);
                foreach (var hitCollider in hitColliders)
                {
                    Health playerHealth = hitCollider.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(healAmount);
                        //Apply speed boost here if you have a movement script
                        // e.g., hitCollider.GetComponent<PlayerMovement>().speed += speedBoost;
                        // You would also need to handle removing the speed boost after 'duration' seconds
                    }
                }
                if (uses <= 0)
                {
                    Debug.Log("Out of uses for " + itemName);
                    return;
                }
                else uses--;
                return;
            }
        }
    }
}
