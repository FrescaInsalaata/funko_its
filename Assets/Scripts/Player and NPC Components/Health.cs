using UnityEngine;

public class Health : MonoBehaviour
{
    public float current = 100f;
    public void TakeDamage(float amount)
    {
        current -= amount;
    }
    public void Heal(float amount)
    {
        current += amount;
    }
}
