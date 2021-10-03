using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLizard : Enemy
{
    [SerializeField] int ammo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootCooldown;
    [SerializeField] float maxShootCooldown;
    [SerializeField] int muzzleFlash;

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
                    SwitchState(EnemyState.Shooting);
                }
                if (stateTimer > 2f)
                {
                    if (!agent.hasPath)
                    {
                        //recalculate
                        Vector3 randomPlace = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
                        agent.SetDestination(randomPlace);
                        SwitchState(EnemyState.Moving);
                    }
                }
                break;

            case EnemyState.Aiming:

                break;

            case EnemyState.Shooting:
                ChangeAnimationState("Attack");
                if (shootCooldown <= 0 && ammo > 0)
                {
                    shootCooldown = maxShootCooldown;
                    Shoot(aimDirection);
                    ammo--;
                    if (ammo <= 0)
                    {
                        SwitchState(EnemyState.Cooldown);
                    }
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
                        SwitchState(EnemyState.Shooting);
                    }
                    else
                    {
                        Vector3 randomPlace = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
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

    protected override void Shoot(Vector3 direction)
    {
        //laser beam
        RaycastHit hit;

        if (muzzleFlashParticles != null) muzzleFlashParticles[muzzleFlash].Play();
        muzzleFlash++;
        if (muzzleFlash > muzzleFlashParticles.Count - 1) muzzleFlash = 0;
        if (Physics.Raycast(shootTransform.position, direction, out hit, 1000))
        {
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(enemyData.weaponDamage);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
        }
    }


}
