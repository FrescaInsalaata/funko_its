using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //GameManager handles area transitions and settings
    public static GameManager Instance;
    private int currentArea = 0;
    private float currentEnemiesToKill;
    public Area[] areas;

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
        if (currentArea + 1 < areas.Length) // check BEFORE increment
        {
            currentArea++;
            areas[currentArea].ActivateArea();
            yield return new WaitForSeconds(4);
            EnemySpawnerManager.Instance.SetEnemiesToSpawn(areas[currentArea].numEnemies);
        }
        else
        {
            Debug.Log("All areas completed!");
            Destroy(EnemySpawnerManager.Instance.gameObject);
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