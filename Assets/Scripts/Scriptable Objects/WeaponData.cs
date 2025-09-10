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

    [Header("Raycast")]
    public float raycastDistance;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public Transform muzzle;

    // Abstract method if you want different weapons to implement shooting
    public virtual void Fire()
    {
        if (!bulletPrefab || muzzle) return;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(muzzle.forward * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}