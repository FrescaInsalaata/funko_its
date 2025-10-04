using UnityEngine;

public enum EnemyType { Punk, Elite, Facebreaker }

public class AttackAI : MonoBehaviour
{
    public WeaponData weapon;
    public EnemyType enemyType;
    public float disengageRange = 20f;
    private float lastAttackTime;
    private EnemyCore enemyCore;
    private AudioSource stabAudio;

    [Header("Elite Specific (EDIT ONLY IF ELITE)")]
    public float retreatDistance = 5f;
    private bool isRetreating = false;

    [Header("Facebreaker Specific (EDIT ONLY IF FACEBREAKER)")]
    public float walkRange = 5f;
    public float chargeRange = 8f;
    public float chargeSpeedMultiplier = 2f;
    public float chargeDuration = 1.3f;
    private bool isCharging = false;
    private float chargeEndTime;

    // Precomputed squared ranges
    private float sqrMeleeRange;
    private float sqrWalkRange;
    private float sqrChargeRange;
    private float sqrDistance;

    private Animator animator;

    void Start()
    {
        enemyCore = GetComponent<EnemyCore>();
        sqrMeleeRange = weapon.range * weapon.range;
        sqrWalkRange = walkRange * walkRange;
        sqrChargeRange = chargeRange * chargeRange;
        animator = GetComponent<Animator>();
        stabAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // If there is no valid target, do nothing
        if (enemyCore.targetPlayer == null)
        {
            enemyCore.agent.isStopped = true;
            animator.SetBool("IsAttacking", false);
            return;
        }

        sqrDistance = (enemyCore.targetPlayer.position - transform.position).sqrMagnitude;

        switch (weapon.weaponType)
        {
            case WeaponType.Melee:
                HandleMeleeAttack(sqrDistance);
                break;
            case WeaponType.Ranged:
                HandleRangedAttack(sqrDistance);
                break;
        }
    }

    bool CanSeeTarget(Transform player)
    {
        Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, enemyCore.detectionRange))
        {
            return hit.transform == player;
        }
        return false;
    }

    void HandleRangedAttack(float sqrDistance)
    {
        if (sqrDistance <= sqrMeleeRange && CanSeeTarget(enemyCore.targetPlayer))
        {
            enemyCore.agent.isStopped = true;
            enemyCore.agent.speed = enemyCore.moveSpeed;

            if (Time.time - lastAttackTime >= weapon.fireRate)
            {
                if (!IsInAttackCooldown())
                {
                    lastAttackTime = Time.time;
                }
                return;
            }
        }
        else
        {
            MoveTowardTarget();
        }
    }

    void HandleMeleeAttack(float sqrDistance)
    {
        if (sqrDistance <= sqrMeleeRange)
        {
            enemyCore.agent.isStopped = true;
            enemyCore.agent.speed = enemyCore.moveSpeed;

            if (!IsInAttackCooldown())
            {
                lastAttackTime = Time.time;
                weapon.MeleeAttack(transform, enemyCore.targetPlayer.gameObject);
                animator.SetBool("IsAttacking", true);
                stabAudio.Play();
                StartCoroutine(ResetAttackAnimation());
            }
            return;
        }
        else
        {
            MoveTowardTarget();
        }
    }

    void MoveTowardTarget()
    {
        enemyCore.agent.isStopped = false;
        enemyCore.agent.speed = enemyCore.moveSpeed;
        enemyCore.agent.SetDestination(enemyCore.targetPlayer.position);
    }
    bool IsInAttackCooldown()
    {
        return Time.time - lastAttackTime < weapon.fireRate;
    }

    System.Collections.IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("IsAttacking", false);
    }
}

