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
    public bool isReloading = false;
    public WeaponType weaponType;
    public BulletType bulletType;

    [Header("Gun")]
    public GameObject projectilePrefab;
    public GameObject weaponPrefab;
    public float projectileSpeed;

    private float currentAmmo;

    // ------------------
    // PUBLIC METHODS
    // ------------------

    public virtual void Fire(GameObject firePoint, int playerID)
    {

        if (projectilePrefab == null || firePoint == null || currentAmmo <= 0 || isReloading) return; //add that i can't fire if it's doing the reload coroutine

        currentAmmo--;

        // Aggiorna UI
        if (UIManager.Instance != null)
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));

        // Instanzia proiettile
        GameObject bullet = Instantiate(projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.transform.forward * projectileSpeed, ForceMode.VelocityChange);
        }
    }

    public void SetMaxAmmo(int playerID)
    {
        currentAmmo = maxAmmo;

        if (playerID >= 0 && UIManager.Instance != null)
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
    }

    public virtual void Reload(MonoBehaviour owner, int playerID)
    {
        if (reloadTime <= 0 || currentAmmo >= maxAmmo || maxAmmo <= 0) return;
        owner.StartCoroutine(ReloadRoutine(playerID));
    }

    public virtual void MeleeAttack(Transform attackPoint, GameObject player)
    {

        player.GetComponent<Health>().TakeDamage(damage);
    }

    // ------------------
    // PRIVATE METHODS
    // ------------------

    private IEnumerator ReloadRoutine(int playerID)
    {
        isReloading = !isReloading;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = !isReloading;

        if (playerID >= 0 && UIManager.Instance != null)
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
    }

}
