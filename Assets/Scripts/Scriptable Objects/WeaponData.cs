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
    public virtual void Fire()
    {
        if (!projectilePrefab || muzzle) return;

        GameObject bullet = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(muzzle.forward * projectileSpeed, ForceMode.VelocityChange);
        }
    }
}