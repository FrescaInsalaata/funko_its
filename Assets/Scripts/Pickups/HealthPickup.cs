using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 50f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}