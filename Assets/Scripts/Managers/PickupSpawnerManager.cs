using UnityEngine;

public class PickupSpawnerManager : MonoBehaviour
{
    public static PickupSpawnerManager Instance;

    [Header("Pickup Prefabs")]
    public GameObject[] pickupGroundPrefabs;
    public GameObject[] weaponGroundPrefabs;
    [Header("Pickup Spawn Points")]
    public GameObject[] pickupSpawnPoints;
    private void Start()
    {
        SpawnAllPickups();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void SpawnAllPickups()
    {
        foreach (var spawnPoint in pickupSpawnPoints)
        {
            if (spawnPoint.tag == "WeaponSpawnPoint")
            {
                if (weaponGroundPrefabs.Length == 0)
                {
                    Debug.LogWarning("No weapon prefabs assigned in PickupSpawnerManager!");
                    continue;
                }
                Debug.Log("Spawning weapon at: " + spawnPoint.name);
                int weaponIndex = Random.Range(0, weaponGroundPrefabs.Length);
                Instantiate(weaponGroundPrefabs[weaponIndex], spawnPoint.transform.position, Quaternion.identity);
                continue;
            }
            else if (spawnPoint.tag == "PickupSpawnPoint")
            {
                if (pickupGroundPrefabs.Length == 0)
                {
                    Debug.LogWarning("No weapon prefabs assigned in PickupSpawnerManager!");
                    continue;
                }
                Debug.Log("Spawning pickup at: " + spawnPoint.name);
                int pickupIndex = Random.Range(0, pickupGroundPrefabs.Length);
                Instantiate(pickupGroundPrefabs[pickupIndex], spawnPoint.transform.position, Quaternion.identity);
                continue;
            }
        }
    }
}
