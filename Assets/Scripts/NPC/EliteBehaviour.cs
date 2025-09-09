using UnityEngine;
using UnityEngine.AI;

public class EliteBehaviour : MonoBehaviour
{
    public enum WeaponType { SMG, SawedOff }
    public WeaponType weaponType;

    [Header("References")]
    public Transform gunMuzzle;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    [Header("Stats")]
    public float moveSpeed = 3f;
    public float detectionRange = 20f;    // Detection radius
    public float minShootRange = 5f;      // Preferred shooting distance
    public float maxShootRange = 15f;     // Max shooting distance
    public float attackCooldown = 1f;
    public float retreatDistance = 5f;    // How far to move away when retreating

    private float lastAttackTime;
    private Transform targetPlayer;
    private NavMeshAgent agent;

    private bool isRetreating = false;

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

            if (isRetreating)
            {
                RetreatFromPlayer();
            }
            else
            {
                if (distance > maxShootRange)
                {
                    // Too far, approach player
                    agent.isStopped = false;
                    agent.SetDestination(targetPlayer.position);
                }
                /*else if (distance < minShootRange)
                {
                    // Too close, retreat
                    isRetreating = true;
                }*/
                else
                {
                    // In shooting range
                    agent.isStopped = true;
                    ShootAtPlayer();
                }
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

    void ShootAtPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown && CanSeeTarget(targetPlayer))
        {
            lastAttackTime = Time.time;

            if (bulletPrefab && gunMuzzle)
            {
                GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation);
                //TO DO: add difference between smg and sawed off
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(gunMuzzle.forward * bulletSpeed, ForceMode.VelocityChange);
                }
            }
        }
    }

    void RetreatFromPlayer()
    {
        Vector3 retreatDir = (transform.position - targetPlayer.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDir * retreatDistance;

        agent.isStopped = false;
        agent.SetDestination(retreatTarget);

        // Once at safe distance, stop retreating
        float distance = Vector3.Distance(transform.position, targetPlayer.position);
        if (distance >= minShootRange + 1f) // Add a little buffer
        {
            isRetreating = false;
        }
    }

    bool CanSeeTarget(Transform player)
    {
        Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
        {
            return hit.transform == player;
        }
        return false;
    }
}
