using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected float currentHP;
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected GameObject playerObject;

    [SerializeField] protected float recalculateTimer;
    [SerializeField] protected float maxRecalculateTimer;
    [SerializeField] protected Transform shootTransform;

    [SerializeField] protected Animator animator;
    [SerializeField] protected string currentAnimState;

    public Action<Enemy> OnEnemyDeath;


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

    [SerializeField] protected EnemyState currentState;
    [SerializeField] protected float stateTimer;
    [SerializeField] protected Vector3 aimDirection;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected List<ParticleSystem> muzzleFlashParticles;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHP = enemyData.maxHP;
        gameObject.name = enemyData.name;
        agent = GetComponent<NavMeshAgent>();
        Vector3 randomPlace = new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
        //agent.SetDestination(randomPlace);

        if (playerObject == null)
        {
            playerObject = GameObject.FindObjectOfType<PlayerController>().gameObject;
        }

        currentState = EnemyState.Cooldown;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        stateTimer += Time.deltaTime;
    }

    protected bool GetLineOfSight(Transform target)
    {
        Debug.LogWarning(target.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(shootTransform.position, target.position - shootTransform.position, out hit))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform == target)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }


    }

    protected void SwitchState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0;
    }

    protected virtual void Shoot(Vector3 direction) { }

    protected void RecalculatePosition()
    {
        agent.SetDestination(new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20)));
    }

    public void Damage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {

            Die();
        }
    }

    protected virtual void Die()
    {
        SwitchState(EnemyState.Death);
        ChangeAnimationState("Die");
        agent.isStopped = true;
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject, 1f);
    }

    protected void ChangeAnimationState(string newState)
    {
        if (animator == null) return;
        if (currentAnimState == newState) return;
        animator.Play(newState);
        currentAnimState = newState;
    }
}
