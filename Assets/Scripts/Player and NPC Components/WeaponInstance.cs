
using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class WeaponInstance
{
    public WeaponData weaponData;
    public float currentAmmo;

    public WeaponInstance(WeaponData data)
    {
        weaponData = data;
        currentAmmo = data.maxAmmo;
    }

    public void Fire(GameObject firePoint, int playerID)
    {
        if (firePoint == null || currentAmmo <= 0 || weaponData.projectilePrefab == null)
        {
            Debug.Log(playerID + " cannot fire: " + (firePoint == null ? "No fire point. " : "") + (currentAmmo <= 0 ? "No ammo. " : "") + (weaponData.projectilePrefab == null ? "No projectile prefab." : ""));
            return;
        }

        currentAmmo--;

        if (UIManager.Instance != null)
        {
            Debug.Log(Mathf.RoundToInt(currentAmmo).ToString());
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
        }

        GameObject bullet = UnityEngine.Object.Instantiate(weaponData.projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour != null)
        {
            bulletBehaviour.GetDamage(weaponData.damage);
        }
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.transform.forward * weaponData.projectileSpeed, ForceMode.VelocityChange);
        }
    }
    public void Reload(MonoBehaviour owner, int playerID)
    {
        if (weaponData.reloadTime <= 0 || currentAmmo >= weaponData.maxAmmo) return;

        owner.StartCoroutine(ReloadRoutine(owner, playerID));
    }

    private IEnumerator ReloadRoutine(MonoBehaviour owner, int playerID)
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        currentAmmo = weaponData.maxAmmo;

        if (UIManager.Instance != null)
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
    }
}
