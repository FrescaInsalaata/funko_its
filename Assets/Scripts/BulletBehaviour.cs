using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    //lifetime of the bullet in seconds
    public float lifetime = 3f;
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
    private void OnCollisionEnter(Collision collision)
    {
        //destroy the bullet
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(20);
            Destroy(gameObject);
        }
    }
}
