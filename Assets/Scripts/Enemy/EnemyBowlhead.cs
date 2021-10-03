using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBowlhead : Enemy
{
    [SerializeField] int ammo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootCooldown;
    [SerializeField] float maxShootCooldown;

    [SerializeField] GameObject laserPrefab;

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
                        Vector3 randomPlace = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
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
                        Vector3 randomPlace = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                        agent.SetDestination(randomPlace);
                        SwitchState(EnemyState.Moving);
                    }
                }
                break;

            default:

                break;
        }

    }

    protected override void Shoot(Vector3 direction)
    {
        //slow moving laser

        if (muzzleFlashParticles != null) muzzleFlashParticles[0].Play();
        GameObject laser = Instantiate(laserPrefab, shootTransform.position, Quaternion.LookRotation(aimDirection, Vector3.up) * Quaternion.Euler(0, 90, 0));
        Projectile projectile = laser.GetComponent<Projectile>();
        projectile.SetProjectileValues(enemyData.weaponDamage, 2f, aimDirection * 8f, 8);
    }
}
