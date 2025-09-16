using Unity.Cinemachine;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int playerID;               // ID unico del Player
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning("No Renderer component found on " + gameObject.name);

        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0f)
            Die();
        else
            UpdateColor();
         UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateColor();
        UpdateUI();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void UpdateColor()
    {
        float t = Mathf.Clamp01(currentHealth / maxHealth); // 1 = full health, 0 = dead
        Color newColor = Color.Lerp(Color.red, Color.green, t); // green at full, red at 0
        if (rend != null)
            rend.material.color = newColor;
    }

    void UpdateUI()
    {
        if (tag != "Player") return;

        if (UIManager.Instance != null)
        {
            // Aggiorna solo il pannello UI corrispondente al playerID
            Debug.Log(Mathf.RoundToInt(currentHealth).ToString());
            UIManager.Instance.updateHealth(playerID, Mathf.RoundToInt(currentHealth));
        }
    }
}
