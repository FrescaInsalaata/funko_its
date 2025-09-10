using UnityEngine;

public class Health : MonoBehaviour
{
    public float current = 100f;
    public void TakeDamage(float amount)
    {
        current -= amount;
        if (current <= 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        current += amount;
        if (current > 100f) current = 100f;
    }
   public void Die()
    {
        Destroy(gameObject);
    }
}
