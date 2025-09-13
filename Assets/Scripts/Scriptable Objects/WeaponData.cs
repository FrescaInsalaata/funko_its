using UnityEngine;
using System.Collections;
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
    public float maxAmmo;
    public float reloadTime;
    public WeaponType weaponType;
    public BulletType bulletType;

    [Header("Gun")]
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    public float projectileSpeed;

    private float currentAmmo;

    // Abstract method if you want different weapons to implement shooting
    public virtual void Fire(GameObject firePoint)
    {
        if (projectilePrefab == null || firePoint == null || currentAmmo <= 0) return;
        currentAmmo--;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.transform.forward * projectileSpeed, ForceMode.VelocityChange);
        }
        Debug.Log("Fired " + weaponName + " from " + firePoint.transform.position + ", Remaining Ammo: " + currentAmmo);
    }
    public void SetMaxAmmo()
    {
        currentAmmo = maxAmmo;
    }
    public virtual void Reload(MonoBehaviour owner)
    {
        if (reloadTime <= 0 || currentAmmo >= maxAmmo || maxAmmo <= 0) return;
        owner.StartCoroutine(ReloadRoutine());
    }
    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }
    public virtual void MeleeAttack(Transform attackPoint, GameObject Player)
    {
        Debug.Log("Melee attack executed at " + attackPoint.position);
        Player.GetComponent<Health>().TakeDamage(damage);
    }
}