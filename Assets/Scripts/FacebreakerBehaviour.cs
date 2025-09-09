using UnityEngine;
using UnityEngine.AI;

public class FacebreakerBehaviour : MonoBehaviour
{
    [Header("Stats")]
    public float health = 300f;
    public float moveSpeed = 2f;           // Slow enemy
    public float detectionRange = 20f;
    public float meleeRange = 3f;          // Melee attack range
    public float rushRange = 8f;           // Medium range to trigger dash
    public float attackCooldown = 2f;
    public float rushSpeedMultiplier = 3f; // Dash speed multiplier
    public float rushDuration = 1f;        // Dash lasts 1 second

    private float lastAttackTime;
    private Transform targetPlayer;
    private NavMeshAgent agent;

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
            Debug.Log($"{name} hits {targetPlayer.name} with a heavy punch!");
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
            Debug.Log($"{name} starts rushing towards {targetPlayer.name}!");
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        EnemySpawnerManager.Instance.ReduceFacebreakerCounter(); // Or specific Facebreaker counter
        Destroy(gameObject);
    }
}
