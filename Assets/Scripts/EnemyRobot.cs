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
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(EnemyState.Aiming);
                }
                break;

            case EnemyState.Aiming:
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
                aimingRenderer.enabled = false;
                Shoot(aimDirection);
                SwitchState(EnemyState.Cooldown);
                break;

            case EnemyState.Cooldown:

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
        }
        else
        {
            laserRenderer.SetPosition(1, shootTransform.position + (direction * 100f));
        }

        Destroy(laserObject, 2f);
    }
}
