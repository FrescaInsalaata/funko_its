using UnityEngine;

public class PickupBehaviour : MonoBehaviour
{
    [Header("Pickup Info")]
    public string itemName;
    public WeaponData weaponData;
    public ItemData itemData;

    void Update()
    {
        //only if name is healthpickup
        if (weaponData == null)
        {
            transform.Rotate(Vector3.up * 20f * Time.deltaTime, Space.World);
            float t = (Mathf.Sin(Time.time) + 1f) / 2f; // oscillates between 0 and 1
            Color color = Color.Lerp(Color.green, Color.yellow, t);
            GetComponent<Renderer>().material.color = color;
        }
    }
}

