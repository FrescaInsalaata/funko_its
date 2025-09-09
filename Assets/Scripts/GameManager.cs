using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int currentArea = 1;
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

    public int GetCurrentArea()
    {
        return currentArea;
    }

    public void SetCurrentArea(int area)
    {
        currentArea = area;
    }
}
