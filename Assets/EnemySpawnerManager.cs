using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; 
    [Header("Area1 Spawn Points")]
    public Transform[] area1SpawnPoints;   // Place empty GameObjects in the scene as spawn points
    [Header("Area2 Spawn Points")]
    public Transform[] area2SpawnPoints;   // Place empty GameObjects in the scene as spawn points
    [Header("Area3 Spawn Points")]
    public Transform[] area3SpawnPoints;   // Place empty GameObjects in the scene as spawn points
    [Header("Area4 Spawn Points")]
    public Transform[] area4SpawnPoints;   // Place empty GameObjects in the scene as spawn points
    [Header("Spawn Settings")]
    public float spawnInterval = 3f;  // Time between spawns

    private float currentArea = 1; // Current area (1 to 4)
    private float timer = 0f;

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
        // Pick a random enemy
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

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
        // Spawn it
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
}