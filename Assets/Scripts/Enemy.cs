using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] float currentHP;
    [SerializeField] EnemyData enemyData;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject playerObject;

    [SerializeField] float recalculateTimer;
    [SerializeField] float maxRecalculateTimer;
    [SerializeField] Transform shootTransform;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] LineRenderer aimingRenderer;

    public enum EnemyState
    {
        Moving,
        Idle,
        Aiming,
        Shooting,
        Cooldown,
        Damage,
        Death
    }

    [SerializeField] EnemyState currentState;
    [SerializeField] float stateTimer;
    [SerializeField] Vector3 aimDirection;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = enemyData.maxHP;
        gameObject.name = enemyData.name;
        agent = GetComponent<NavMeshAgent>();
        aimingRenderer = GetComponent<LineRenderer>();
        Vector3 randomPlace = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
        agent.SetDestination(randomPlace);

        if (playerObject == null)
        {
            playerObject = GameObject.FindObjectOfType<PlayerController>().gameObject;
        }

        currentState = EnemyState.Aiming;
    }

    // Update is called once per frame
    void Update()
    {
        stateTimer += Time.deltaTime;
        switch (currentState)
        {
            case EnemyState.Aiming:
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
                    //TODO: check if they need to move
                    aimingRenderer.enabled = true;
                    SwitchState(EnemyState.Aiming);
                }
                break;

            default:

                break;
        }

        /*
        recalculateTimer += Time.deltaTime;
        if (recalculateTimer >= maxRecalculateTimer)
        {
            if (currentState == EnemyState.Shooting)
            {
                Shoot();
            }
            else
            {
                RecalculatePosition();
            }
            recalculateTimer -= maxRecalculateTimer;
        }
        */
    }

    void SwitchState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0;
    }

    void Shoot(Vector3 direction)
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
        }
        else
        {
            laserRenderer.SetPosition(1, shootTransform.position + (direction * 100f));
        }




        Destroy(laserObject, 2f);
    }

    void RecalculatePosition()
    {
        agent.SetDestination(new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)));
    }

    public void Damage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //eventually do animation in here yeet
        Destroy(gameObject);
    }
}
