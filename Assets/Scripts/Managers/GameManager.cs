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
        areas[0].ActivateArea();
        //get number of players from PlayerManager
        int numPlayers = FindAnyObjectByType<PlayerManager>().playerCount;
        EnemySpawnerManager.Instance.SetEnemiesToSpawn(areas[0].numEnemies, numPlayers);
    }

    public void CompletedArea()
    {
        if (currentArea + 1 < areas.Length) // check BEFORE increment
        {
            currentArea++;
            Debug.Log("Opening area " + areas[currentArea].areaName);
            areas[currentArea].ActivateArea();
            int numPlayers = FindAnyObjectByType<PlayerManager>().playerCount;
            EnemySpawnerManager.Instance.SetEnemiesToSpawn(areas[currentArea].numEnemies, numPlayers);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");
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