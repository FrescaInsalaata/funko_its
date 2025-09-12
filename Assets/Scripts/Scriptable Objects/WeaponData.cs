using UnityEngine;
public enum WeaponType { Melee, Ranged }
public enum BulletType { Raycast, Projectile, None }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public float damage;
    public float fireRate;
    public float range;
    public WeaponType weaponType;
    public BulletType bulletType;


    [Header("Gun")]
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    public float projectileSpeed;
    public Transform muzzle;

    // Abstract method if you want different weapons to implement shooting
    public virtual void Fire(Transform firePoint)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.VelocityChange);
        }
    }

    public virtual void MeleeAttack(Transform attackPoint, GameObject Player)
    {
        Debug.Log("Melee attack executed at " + attackPoint.position);
        Player.GetComponent<Health>().TakeDamage(damage);
    }
}