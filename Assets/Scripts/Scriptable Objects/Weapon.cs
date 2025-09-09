using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    // Optional: store other data like damage, range, spread
    public float damage = 10f;
    public float range = 15f;

    // Abstract method if you want different weapons to implement shooting
    public virtual void Fire(Transform firePoint)
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}