using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCore : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    private float timerChase = 0f;
    [HideInInspector] public bool isFighting = false;
    [HideInInspector] public Transform targetPlayer;
    [HideInInspector] public NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        timerChase += Time.deltaTime;
        if(timerChase >= 0.35f)
        {
            timerChase = 0f;
            FindClosestPlayer();
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        targetPlayer = null;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                targetPlayer = player.transform;
            }
        }

        if (minDistance > detectionRange)
        {
            isFighting = false;
        }
        else
        {
            isFighting = true;
        }
    }
}
