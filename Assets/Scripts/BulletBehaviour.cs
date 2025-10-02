using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    //lifetime of the bullet in seconds
    public float lifetime = 3f;
    private float bulletDamage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
    }
    //on collision with another object
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(collision.gameObject.name + " now has health.");
            var enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth == null)
            {
                Debug.LogWarning(collision.gameObject.name + " has no Health component!");
                return;
            }
            enemyHealth.TakeDamage(bulletDamage);
            Debug.Log("FUNZIONA!");
        }
        Destroy(gameObject);
    }

    public void SetDamage(float damage)
    {
        bulletDamage = damage;
    }

    public void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }
}
