using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnerManager : MonoBehaviour
{
    // EnemySpawnerManager
    // Spawns enemies based on the current area
    // Limits the number of enemies in the scene
    // Uses empty GameObjects as spawn points

    public static EnemySpawnerManager Instance;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;
    // TODO: Place empty GameObjects in the scene as spawn points
    [Header("Area0 Spawn Points")]
    public GameObject[] area0SpawnPoints;
    [Header("Area1 Spawn Points")]
    public GameObject[] area1SpawnPoints;
    [Header("Area2 Spawn Points")]
    public GameObject[] area2SpawnPoints;
    [Header("Area3 Spawn Points")]
    public GameObject[] area3SpawnPoints;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;  // Time between spawns

    private float timer = 0f; // Timer to track spawn intervals
    private float enemiesToSpawn = 0; // Enemies left to spawn in the current area
    private float currentFacebreakers = 0; // Current number of Facebreakers in the scene

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
        if (enemiesToSpawn <= 0 && !IsAnyEnemyAlive())
        {
            GameManager.Instance.CompletedArea();
          
        }
    }
    void SpawnEnemy()
    {
        if (enemiesToSpawn <= 0)
            return;

        enemiesToSpawn--;

        int currentArea = GameManager.Instance.GetCurrentArea();
        GameObject enemyToSpawn = null;

        // Pick enemy until we find one valid to spawn
        for (int i = 0; i < 5; i++) // try a few times max
        {
            GameObject candidate = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (candidate.name == "BossEnemyFBX" && currentArea < 2)
                continue;
            enemyToSpawn = candidate;
            break;
        }

        if (enemyToSpawn == null)
        {
            Debug.LogWarning("Could not find a valid enemy to spawn this round.");
            return;
        }

        Transform spawnPoint = null;
        switch (currentArea)
        {
            case 0:

                spawnPoint = area0SpawnPoints[Random.Range(0, area0SpawnPoints.Length)].transform;
                break;
            case 1:

                spawnPoint = area1SpawnPoints[Random.Range(0, area1SpawnPoints.Length)].transform;
                break;
            case 2:

                spawnPoint = area2SpawnPoints[Random.Range(0, area2SpawnPoints.Length)].transform;
                break;
            case 3:

                spawnPoint = area3SpawnPoints[Random.Range(0, area3SpawnPoints.Length)].transform;
                break;
        }

        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
    }
    public void SetEnemiesToSpawn(float enemies, float numPlayers) // Called by GameManager when area starts
    {
        if (enemies <= 0 || numPlayers <= 0)
        {
            numPlayers = 1;
        }
        enemiesToSpawn = enemies * numPlayers;
    }
    public void ReduceFacebreakerCounter() // Called by Facebreaker on death
    {
        if (currentFacebreakers > 0)
            currentFacebreakers--;
    }
    private bool IsAnyEnemyAlive()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length > 0;
    }
}

