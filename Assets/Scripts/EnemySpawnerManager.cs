using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public static EnemySpawnerManager Instance;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    // WARNING! Place empty GameObjects in the scene as spawn points
    [Header("Area1 Spawn Points")]
    public Transform[] area1SpawnPoints;
    [Header("Area2 Spawn Points")]
    public Transform[] area2SpawnPoints;
    [Header("Area3 Spawn Points")]
    public Transform[] area3SpawnPoints;
    [Header("Area4 Spawn Points")]
    public Transform[] area4SpawnPoints;
    [Header("Spawn Settings")]
    public float spawnInterval = 3f;  // Time between spawns
    private float timer = 0f;
    private float currentPunks;
    private float currentElites;
    private float currentFacebreakers;
    private const float MAX_ENEMIES = 30f;
    private const float MAX_FACEBREAKERS = 2f;

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
    }

    void SpawnEnemy()
    {
        if (currentEnemies() >= MAX_ENEMIES)
            return;
        // Gets current area from GameManager
        float currentArea = GameManager.Instance.GetCurrentArea();
        // Pick a random enemy
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        switch (enemyToSpawn)
        {
            case var _ when enemyToSpawn.name.Contains("Punk"):
                IncreasePunkCounter();
                break;
            case var _ when enemyToSpawn.name.Contains("Elite"):
                IncreaseEliteCounter();
                break;
            case var _ when enemyToSpawn.name.Contains("Facebreaker"):
                if (currentFacebreakers >= MAX_FACEBREAKERS)
                    return;
                IncreaseFacebreakerCounter();
                break;
            default:
                break;
        }

        // Pick a random spawn point
        Transform spawnPoint = null;
        switch (currentArea)
        {
            case 1:
                spawnPoint = area1SpawnPoints[Random.Range(0, area1SpawnPoints.Length)];
                break;
            case 2:
                spawnPoint = area2SpawnPoints[Random.Range(0, area2SpawnPoints.Length)];
                break;
            case 3:
                spawnPoint = area3SpawnPoints[Random.Range(0, area3SpawnPoints.Length)];
                break;
            case 4:
                spawnPoint = area4SpawnPoints[Random.Range(0, area4SpawnPoints.Length)];
                break;
            default:
                break;
        }
        //Spawn it
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }

    public void ReducePunkCounter()
    {
        if (currentPunks > 0)
            currentPunks--;
    }
    private void IncreasePunkCounter()
    {
        currentPunks++;
    }
    public void ReduceEliteCounter()
    {
        if (currentElites > 0)
            currentElites--;
    }
    private void IncreaseEliteCounter()
    {
        currentElites++;
    }
    public void ReduceFacebreakerCounter()
    {
        if (currentFacebreakers > 0)
            currentFacebreakers--;
    }
    private void IncreaseFacebreakerCounter()
    {
        currentFacebreakers++;
    }
    private float currentEnemies()
    {
        return currentPunks + currentElites + currentFacebreakers;
    }
}

