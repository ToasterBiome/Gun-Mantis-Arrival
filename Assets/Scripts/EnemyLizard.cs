using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLizard : Enemy
{
    [SerializeField] int ammo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootCooldown;

    [SerializeField] float maxShootCooldown;

    protected override void Start()
    {
        base.Start();
        ammo = maxAmmo;
    }

    protected override void Update()
    {
        base.Update();
        shootCooldown -= Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.Moving:
                if (shootCooldown <= 0 && ammo > 0)
                {
                    shootCooldown = maxShootCooldown;
                    Shoot(playerObject.transform.position - transform.position);
                    ammo--;
                    if (ammo <= 0)
                    {
                        SwitchState(EnemyState.Cooldown);
                    }
                }
                break;

            case EnemyState.Aiming:

                break;

            case EnemyState.Shooting:

                break;

            case EnemyState.Cooldown:
                if (stateTimer > 2f)
                {
                    ammo = maxAmmo;
                    bool lineOfSlight = GetLineOfSight(playerObject.transform);
                    if (lineOfSlight)
                    {
                        SwitchState(EnemyState.Moving);
                    }
                    else
                    {
                        Vector3 randomPlace = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
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
        if (Physics.Raycast(shootTransform.position, direction, out hit))
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
