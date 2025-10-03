using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    public int playerID;               // ID unico del Player
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    private Renderer rend;
    private static readonly int DeathHash = Animator.StringToHash("Death");

    void Start()
    {
        if (tag != "Player")
            playerID = -1;

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
        {
            if (tag != "Player") //if tag isn't player, stop it from moving
            {
                if (GetComponent<AttackAI>() != null) //remove component
                    Destroy(GetComponent<AttackAI>());

            }
            if (tag == "Player")
            {
                if (GetComponent<PlayerBehaviour>().playerInput != null)
                    GetComponent<PlayerBehaviour>().playerInput.enabled = false;
                // Find the CinemachineVirtualCamera in the scene
                CinemachineCamera vcam = FindObjectOfType<CinemachineCamera>();
                if (vcam != null)
                {
                    vcam.enabled = false;
                }
            }
            Die();
        }

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
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().SetTrigger(DeathHash);
        if (tag == "Player")
        {
            if (GetComponent<PlayerBehaviour>().playerInput != null)
                GetComponent<PlayerBehaviour>().playerInput.enabled = false;
        }
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject, 3f);
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
