using UnityEngine;

public class PickupBehaviour : MonoBehaviour
{
    [Header("Pickup Info")]
    public string itemName;
    public WeaponData weaponData;
    public ItemData itemData;

    void Update()
    {
        if (weaponData == null)
        {
            if (itemName == "HealthPickup" || itemName == "VibeCheckPickup")
            {
                transform.Rotate(Vector3.up * 20f * Time.deltaTime, Space.World);
                float t = (Mathf.Sin(Time.time) + 1f) / 2f;

                Color color = itemName == "HealthPickup"
                    ? Color.Lerp(Color.green, Color.yellow, t)
                    : Color.Lerp(Color.blue, Color.cyan, t);

                var rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    // Works for URP/Lit
                    if (rend.material.HasProperty("_BaseColor"))
                        rend.material.SetColor("_BaseColor", color);
                    else
                        rend.material.color = color;  // fallback for legacy shaders
                }
            }
        }
    }
}

