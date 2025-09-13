using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    private Renderer rend;
    public void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("No Renderer component found on " + gameObject.name);
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        } else
        {
            currentHealth = Mathf.Max(currentHealth, 0);
            UpdateColor();
        }
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > 100f) currentHealth = 100f;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateColor();
    }
   public void Die()
   {
       Destroy(gameObject);
   }

   void UpdateColor()
   {
       float t = Mathf.Clamp01(currentHealth / maxHealth); // 1 = full health, 0 = dead
       Color newColor = Color.Lerp(Color.red, Color.green, t); // green at full, red at 0
       rend.material.color = newColor;
   }
}
