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
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
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
        //wait 3 seconds, then change to lose scene
        StartCoroutine(WaitAndChangeScene(3f));
    }

    private System.Collections.IEnumerator WaitAndChangeScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (tag == "Player")
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
        else
            Destroy(gameObject);
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
