using UnityEngine;

public enum EnemyType { Punk, Elite, Facebreaker }

public class AttackAI : MonoBehaviour
{
    public WeaponData weapon;
    public EnemyCore enemyCore;
    public EnemyType enemyType;
    private float lastAttackTime;

    // Elite Specific
    private bool isRetreating = false;
    private float retreatDistance = 5f;

    // Facebreaker Specific
    public float walkRange = 5f;
    public float chargeRange = 8f;
    public float chargeSpeedMultiplier = 3f;
    public float chargeDuration = 1f;
    private bool isCharging = false;
    private float chargeEndTime;

    // Precomputed squared ranges
    private float sqrMeleeRange;
    private float sqrWalkRange;
    private float sqrChargeRange;

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
        if (enemyCore.targetPlayer == null)
        {
            enemyCore.agent.isStopped = true;
            return;
        }

        // Compute squared distance once
        float sqrDistance = (enemyCore.targetPlayer.position - transform.position).sqrMagnitude;

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

            if (Time.time - lastAttackTime >= weapon.fireRate)
            {
                lastAttackTime = Time.time;
                weapon.Fire();

                if (enemyType == EnemyType.Elite)
                    HandleEliteRetreat();
            }
        }
        else
        {
            enemyCore.agent.isStopped = false;
            enemyCore.agent.speed = enemyCore.moveSpeed;
            enemyCore.agent.SetDestination(enemyCore.targetPlayer.position);
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

            if (Time.time - lastAttackTime >= weapon.fireRate)
            {
                lastAttackTime = Time.time;
                // TODO: Melee attack implementation
            }
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
            enemyCore.agent.isStopped = false;
            enemyCore.agent.speed = enemyCore.moveSpeed;
            enemyCore.agent.SetDestination(enemyCore.targetPlayer.position);
        }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeEndTime = Time.time + chargeDuration;
        enemyCore.agent.isStopped = false;
        enemyCore.agent.speed = enemyCore.moveSpeed * chargeSpeedMultiplier;
        enemyCore.agent.SetDestination(enemyCore.targetPlayer.position);
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
}