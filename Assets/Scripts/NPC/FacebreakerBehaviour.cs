using UnityEngine;
using UnityEngine.AI;

public class FacebreakerBehaviour : MonoBehaviour
{
    [Header("General Stats")]
    public float moveSpeed = 2f;
    public float detectionRange = 20f;
    public float attackCooldown = 2f;

    public float meleeRange = 3f;

    //Facebreaker has heavy melee attacks
    public float heavyAttackMultiplier = 2f;

    private float lastAttackTime;
    private Transform targetPlayer;
    private NavMeshAgent agent;

    //Facebreaker Specific Behaviour
    [Header("Rush Behaviour")]
    public float rushRange = 8f;
    public float rushSpeedMultiplier = 3f;
    public float rushDuration = 1f;
    private bool isRushing = false;
    private float rushEndTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        FindClosestPlayer();
        if (targetPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            if (isRushing)
            {
                // During rush, just move straight towards target
                if (Time.time >= rushEndTime)
                {
                    isRushing = false;
                    agent.speed = moveSpeed;
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(targetPlayer.position);
                    return; // Skip normal melee logic during rush
                }
            }

            if (distance <= meleeRange)
            {
                agent.isStopped = true;
                TryMeleeAttack();
            }
            else if (distance <= rushRange)
            {
                StartRush();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(targetPlayer.position);
            }
        }
        else
        {
            agent.isStopped = true; // Idle if no target
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
            targetPlayer = null;
    }

    void TryMeleeAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            // TODO: Apply heavy damage to player
        }
    }

    void StartRush()
    {
        if (!isRushing)
        {
            isRushing = true;
            rushEndTime = Time.time + rushDuration;
            agent.speed = moveSpeed * rushSpeedMultiplier;
            agent.isStopped = false;
        }
    }
}
