using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class WeaponInstance
{
    public WeaponData weaponData;
    public float currentAmmo;
    private bool isReloading = false;

    public WeaponInstance(WeaponData data)
    {
        weaponData = data;
        currentAmmo = data.maxAmmo;
    }

    public void Fire(GameObject firePoint, int playerID, Color playerColor, bool isVibeCheckActive)
    {
        if (firePoint == null || currentAmmo <= 0 || weaponData.projectilePrefab == null || isReloading)
        {
            Debug.Log(playerID + " cannot fire: " + (firePoint == null ? "No fire point. " : "") + (currentAmmo <= 0 ? "No ammo. " : "") + (weaponData.projectilePrefab == null ? "No projectile prefab." : ""));
            return;
        }

        currentAmmo--;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
        }

        AudioSource audioSource = firePoint.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on " + firePoint.name);
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, weaponData.fireSounds.Length);
            audioSource.clip = weaponData.fireSounds[randomIndex];
            audioSource.Play();
        }

        GameObject bullet = UnityEngine.Object.Instantiate(weaponData.projectilePrefab, firePoint.transform.position, firePoint.transform.rotation);
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour != null)
        {
            if (isVibeCheckActive)
            {
                bulletBehaviour.SetDamage(weaponData.damage * 2);
            }
            else
            {
                bulletBehaviour.SetDamage(weaponData.damage);
            }
            bulletBehaviour.SetColor(playerColor); 
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
        isReloading = true;
        AudioSource audioSource = owner.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = owner.gameObject.AddComponent<AudioSource>();
        }
        if (weaponData.reloadSound != null)
        {
            audioSource.clip = weaponData.reloadSound;
            audioSource.Play();
        }
        owner.StartCoroutine(ReloadRoutine(owner, playerID));
    }

    private IEnumerator ReloadRoutine(MonoBehaviour owner, int playerID)
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        currentAmmo = weaponData.maxAmmo;

        if (UIManager.Instance != null)
            UIManager.Instance.updateAmmo(playerID, Mathf.RoundToInt(currentAmmo));
        isReloading = false;
    }
}
