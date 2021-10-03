using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : Enemy
{
    [SerializeField] GameObject laserPrefab;
    [SerializeField] LineRenderer aimingRenderer;

    protected override void Start()
    {
        base.Start();
        aimingRenderer = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        switch (currentState)
        {
            case EnemyState.Moving:
                ChangeAnimationState("Walk");
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(EnemyState.Aiming);
                }
                break;

            case EnemyState.Aiming:
                ChangeAnimationState("Aim");
                aimingRenderer.enabled = true;
                aimDirection = Vector3.Lerp(aimDirection, playerObject.transform.position - transform.position, Time.deltaTime * 2f);
                aimingRenderer.SetPosition(0, shootTransform.position);
                aimingRenderer.SetPosition(1, aimDirection + (aimDirection * 100f));
                if (stateTimer > 2f && Vector3.Angle(aimDirection, playerObject.transform.position - transform.position) < 5f)
                {
                    SwitchState(EnemyState.Shooting);
                }
                break;

            case EnemyState.Shooting:
                ChangeAnimationState("Attack");
                aimingRenderer.enabled = false;
                Shoot(aimDirection);
                SwitchState(EnemyState.Cooldown);
                break;

            case EnemyState.Cooldown:
                ChangeAnimationState("Attack");
                if (stateTimer > 2f)
                {
                    bool lineOfSlight = GetLineOfSight(playerObject.transform);
                    Debug.LogWarning(lineOfSlight);
                    if (lineOfSlight)
                    {
                        SwitchState(EnemyState.Aiming);
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
        GameObject laserObject;
        Debug.Log("Shooting");
        laserObject = Instantiate(laserPrefab, shootTransform.position, Quaternion.identity);
        if (muzzleFlashParticles.Count > 0) muzzleFlashParticles[0].Play();
        LineRenderer laserRenderer = laserObject.GetComponent<LineRenderer>();
        laserRenderer.SetPosition(0, shootTransform.position);
        if (Physics.Raycast(shootTransform.position, direction, out hit))
        {
            //spawn particles or somethin
            laserRenderer.SetPosition(1, hit.point);

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(enemyData.weaponDamage);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 1f);
        }
        else
        {
            laserRenderer.SetPosition(1, shootTransform.position + (direction * 100f));
        }

        Destroy(laserObject, 2f);
    }

    protected override void Die()
    {
        base.Die();
        Destroy(aimingRenderer);
    }
}
