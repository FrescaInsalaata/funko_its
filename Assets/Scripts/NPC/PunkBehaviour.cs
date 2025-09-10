using UnityEngine;
using UnityEngine.AI;

public class PunkBehaviour : MonoBehaviour
{
    [Header("General Stats")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    public float attackCooldown = 1.5f;

    public float meleeRange = 2f;

    //Punk has either a knife or a pistol
    public enum WeaponType { Knife, Pistol }
    public WeaponType weaponType;

    [Header("Ranged Behaviour")]
    public float shootRange = 5f;
    public Transform gunMuzzle;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    private float lastAttackTime;
    private Transform targetPlayer;
    private NavMeshAgent agent;

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

            switch (weaponType)
            {
                case WeaponType.Knife:
                    HandleKnifeBehavior(distance);
                    break;
                case WeaponType.Pistol:
                    HandlePistolBehavior(distance);
                    break;
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
            targetPlayer = null; // Out of detection range
    }

    void HandleKnifeBehavior(float distance)
    {
        if (distance <= meleeRange)
        {
            agent.isStopped = true;
            TryMeleeAttack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }
    }

    void HandlePistolBehavior(float distance)
    {
        if (distance <= shootRange && CanSeeTarget(targetPlayer))
        {
            agent.isStopped = true;
            TryShoot();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }
    }

    void TryMeleeAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            // TODO: Apply damage to player
        }
    }

    void TryShoot()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            if (bulletPrefab && gunMuzzle)
            {
                GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation);

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Ensure physics is active
                    rb.linearVelocity = Vector3.zero; // Reset any prefab velocity
                    rb.AddForce(gunMuzzle.forward * bulletSpeed, ForceMode.VelocityChange);
                }
            }
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
