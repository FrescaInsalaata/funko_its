using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    //This EnemySpawnerManager spawns enemies based on the current area
    //It also limits the number of enemies in the scene
    //It uses empty GameObjects as spawn points

    public static EnemySpawnerManager Instance;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    // WARNING! Place empty GameObjects in the scene as spawn points
    [Header("Area0 Spawn Points")]
    public Transform[] area0SpawnPoints;
    [Header("Area1 Spawn Points")]
    public Transform[] area1SpawnPoints;
    [Header("Area2 Spawn Points")]
    public Transform[] area2SpawnPoints;
    [Header("Area3 Spawn Points")]
    public Transform[] area3SpawnPoints;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;  // Time between spawns
    private float enemiesToSpawn = 0; // Enemies left to spawn in the current area
    private float timer = 0f; // Timer to track spawn intervals
    private float currentFacebreakers = 0; // Current number of Facebreakers in the scene
    private const float MAX_FACEBREAKERS = 2f; // Max number of Facebreakers allowed in the scene

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
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
        if (enemiesToSpawn <= 0)
        {
            StartCoroutine(GameManager.Instance.CompletedArea());
        }
    }
    void SpawnEnemy()
    {
        if (enemiesToSpawn <= 0)
            return;
        enemiesToSpawn--;

        float currentArea = GameManager.Instance.GetCurrentArea(); // Gets current area from GameManager
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; // Pick a random enemy

        if (currentFacebreakers < MAX_FACEBREAKERS)
        {
            currentFacebreakers++;
        } else return;

        //Spawn in current area
        Transform spawnPoint = null;
        switch (currentArea)
        {
            case 0:
                spawnPoint = area0SpawnPoints[Random.Range(0, area0SpawnPoints.Length)];
                break;
            case 1:
                spawnPoint = area1SpawnPoints[Random.Range(0, area1SpawnPoints.Length)];
                break;
            case 2:
                spawnPoint = area2SpawnPoints[Random.Range(0, area2SpawnPoints.Length)];
                break;
            case 3:
                spawnPoint = area3SpawnPoints[Random.Range(0, area3SpawnPoints.Length)];
                break;
            default:
                break;
        }
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation); //Spawn it

    }
    public void SetEnemiesToSpawn(float enemies)
    {
        enemiesToSpawn = enemies;
    }
    public void ReduceFacebreakerCounter()
    {
        if (currentFacebreakers > 0)
            currentFacebreakers--;
    }
}

