using UnityEngine;

public enum EnemyType { Punk, Elite, Facebreaker }

public class AttackAI : MonoBehaviour
{
    public WeaponData weapon;
    public EnemyType enemyType;
    public float disengageRange = 20f;
    private float lastAttackTime;
    private EnemyCore enemyCore;

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

    void Start()
    {
        enemyCore = GetComponent<EnemyCore>();

        // Precompute squared ranges for efficiency
        sqrMeleeRange = weapon.range * weapon.range;
        sqrWalkRange = walkRange * walkRange;
        sqrChargeRange = chargeRange * chargeRange;
    }

    void Update()
    {
        if (enemyCore.targetPlayer == null || !enemyCore.isFighting)
        {
            if (enemyCore.agent.isOnNavMesh)
            {
                enemyCore.agent.isStopped = true;
                enemyCore.agent.ResetPath(); // stop movement completely
            }
            sqrDistance = Mathf.Infinity; // avoid any movement logic
            return;
        } //Idle
        
        if ((enemyCore.targetPlayer.position - transform.position).sqrMagnitude > disengageRange * disengageRange)
        {
            enemyCore.isFighting = false;
            enemyCore.agent.isStopped = true;
            enemyCore.agent.ResetPath();
            return;
        } //Disengage

        sqrDistance = (enemyCore.targetPlayer.position - transform.position).sqrMagnitude;

        switch (weapon.weaponType)
        {
            case WeaponType.Melee:
                HandleMeleeAttack(sqrDistance); // Handle melee attack logic
                break;
            case WeaponType.Ranged:
                HandleRangedAttack(sqrDistance); //
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
                    weapon.Fire();
                    if (enemyType == EnemyType.Elite)
                       HandleEliteRetreat();
                }

                return; // skip movement logic while attacking
            }
        }
        else
        {
            MoveTowardTarget();
        }
    }

    void HandleMeleeAttack(float sqrDistance)
    {
        if (isCharging)
        {
            ContinueCharge();
            return;
        }

        if (sqrDistance <= sqrMeleeRange) // Player in melee range
        {
            enemyCore.agent.isStopped = true;
            enemyCore.agent.speed = enemyCore.moveSpeed;

            if (!IsInAttackCooldown())
            {
                lastAttackTime = Time.time;
                // TODO: Melee attack implementation
            }

            // Keep the agent stopped during the cooldown
            return;
        }
        else if (enemyType == EnemyType.Facebreaker &&
                 sqrDistance > sqrWalkRange &&
                 sqrDistance <= sqrChargeRange &&
                 !isCharging)
        {
            StartCharge();
        }
        else // Walk towards player
        {
            MoveTowardTarget();
        }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeEndTime = Time.time + chargeDuration;
        MoveTowardTarget();
    }

    void ContinueCharge()
    {
        enemyCore.agent.SetDestination(enemyCore.targetPlayer.position);

        if (Time.time >= chargeEndTime)
        {
            isCharging = false;
            enemyCore.agent.speed = enemyCore.moveSpeed;
        }
    }

    void HandleEliteRetreat()
    {
        if (!isRetreating)
        {
            isRetreating = true;
            Vector3 retreatDir = (transform.position - enemyCore.targetPlayer.position).normalized;
            Vector3 retreatTarget = transform.position + retreatDir * retreatDistance;
            enemyCore.agent.isStopped = false;
            enemyCore.agent.SetDestination(retreatTarget);
        }
        else
        {
            float currentDistance = (enemyCore.targetPlayer.position - transform.position).sqrMagnitude;
            if (currentDistance >= sqrMeleeRange)
                isRetreating = false;
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
}

