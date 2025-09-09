using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //GameManager handles area transitions and settings
    public static GameManager Instance;
    [Header("Area Settings")]
    public int currentArea = 0;
    public Area[] areas;
    public float currentEnemiesToKill;

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
    private void Start()
    {
        InitializeAreas();
    }
    void InitializeAreas()
    {
        for (int i = 0; i < areas.Length; i++)
        {
            areas[i].BuildArea();
        }
        areas[0].ActivateArea(); // Activate first area
        EnemySpawnerManager.Instance.SetEnemiesToSpawn(areas[0].numEnemies);
    }
    
    public IEnumerator CompletedArea()
    {
        if (currentArea < areas.Length)
        {
            currentArea++;
            areas[currentArea].ActivateArea(); // Activate next area
            yield return new WaitForSeconds(4);
            EnemySpawnerManager.Instance.SetEnemiesToSpawn(areas[currentArea].numEnemies);
        }
    }
    public int GetCurrentArea()
    {
        return currentArea;
    }
}

[System.Serializable]
public class Area
{
    public string areaName;
    public GameObject[] doors;
    public Light[] lights;
    public float numEnemies;
    public void BuildArea()
    {
        foreach (var light in lights)
            light.enabled = false;
        foreach (var door in doors)
            door.SetActive(true); //Close doors
    }
    public void ActivateArea()
    {
        foreach (var light in lights)
            light.enabled = true;
        foreach (var door in doors)
            door.SetActive(false); //Open doors
    }
}