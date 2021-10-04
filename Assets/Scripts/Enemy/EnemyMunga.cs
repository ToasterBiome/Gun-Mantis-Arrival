using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMunga : Enemy
{
    [SerializeField] int ammo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootCooldown;
    [SerializeField] float maxShootCooldown;
    [SerializeField] int muzzleFlash;

    [SerializeField] float meleeRadius;

    protected override void Start()
    {
        base.Start();
        ammo = maxAmmo;
    }

    protected override void Update()
    {
        base.Update();
        shootCooldown -= Time.deltaTime;
        aimDirection = Vector3.Lerp(aimDirection, playerObject.transform.position - transform.position, Time.deltaTime * 4f);
        switch (currentState)
        {
            case EnemyState.Moving:
                ChangeAnimationState("Walk");
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(EnemyState.Aiming);
                }
                if (stateTimer > 2f)
                {
                    if (!agent.hasPath)
                    {
                        //recalculate
                        Vector3 randomPlace = playerObject.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                        agent.SetDestination(randomPlace);
                        SwitchState(EnemyState.Moving);
                    }
                }
                break;

            case EnemyState.Aiming:
                ChangeAnimationState("Idle");
                if (Vector3.Distance(transform.position, playerObject.transform.position) <= meleeRadius + 1f)
                {
                    SwitchState(EnemyState.Shooting);
                }
                else
                {
                    Vector3 randomPlace = playerObject.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                    agent.SetDestination(randomPlace);
                    SwitchState(EnemyState.Moving);
                }
                break;

            case EnemyState.Shooting:
                ChangeAnimationState("Attack");
                if (shootCooldown <= 0)
                {
                    MeleeAttack();
                    shootCooldown += maxShootCooldown;
                }
                if (stateTimer >= 1f)
                {
                    SwitchState(EnemyState.Cooldown);
                }
                break;

            case EnemyState.Cooldown:
                ChangeAnimationState("Idle");
                if (stateTimer > 2f)
                {
                    ammo = maxAmmo;
                    bool lineOfSlight = GetLineOfSight(playerObject.transform);
                    if (lineOfSlight)
                    {
                        SwitchState(EnemyState.Aiming);
                    }
                    else
                    {
                        Vector3 randomPlace = playerObject.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                        agent.SetDestination(randomPlace);
                        SwitchState(EnemyState.Moving);
                    }
                    //TODO: check if they need to move
                }
                break;

            default:

                break;
        }

    }

    void MeleeAttack()
    {
        base.Shoot(Vector3.zero);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, meleeRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damagable = hitCollider.GetComponent<IDamageable>();
            if (damagable != null)
            {
                if (hitCollider.gameObject.layer == 6)
                {
                    damagable.Damage(enemyData.weaponDamage);
                }
                else
                {
                    if (hitCollider.gameObject != gameObject)
                    { //don't hit self
                        damagable.Damage(enemyData.weaponDamage * 0.1f);
                    }

                }

            }
        }
    }

}
