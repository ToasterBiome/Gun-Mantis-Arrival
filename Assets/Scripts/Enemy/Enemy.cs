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
    [SerializeField] GameObject _player;
    protected GameObject playerObject
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindObjectOfType<PlayerController>().gameObject;
            }
            return _player;
        }
    }

    [SerializeField] protected float recalculateTimer;
    [SerializeField] protected float maxRecalculateTimer;
    [SerializeField] protected Transform shootTransform;

    [SerializeField] protected Animator animator;
    [SerializeField] protected string currentAnimState;
    public Action<Enemy> OnEnemyDeath;
    [SerializeField] AudioSource audioSource;

    [SerializeField] SpriteRenderer spriteRenderer;


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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Vector3 randomPlace = new Vector3(UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
        //agent.SetDestination(randomPlace);
        currentState = EnemyState.Cooldown;
        Material spriteMaterial = spriteRenderer.material;
        LeanTween.value(spriteRenderer.gameObject, 1f, 0f, 1f).setOnUpdate((value) =>
            {
                spriteMaterial.SetFloat("_DissolveAmount", value);
            });
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        stateTimer += Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(aimDirection);
    }

    protected bool GetLineOfSight(Transform target)
    {
        Debug.LogWarning(target.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(shootTransform.position, target.position - shootTransform.position, out hit, 1000))
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

    protected virtual void Shoot(Vector3 direction)
    {
        audioSource.clip = enemyData.fireSound;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

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

    public void Heal(float amount, bool allowOverheal)
    {
        currentHP += amount;
        if (!allowOverheal)
        {
            if (currentHP > enemyData.maxHP)
            {
                currentHP = enemyData.maxHP;
            }
        }
    }

    protected virtual void Die()
    {
        SwitchState(EnemyState.Death);
        ChangeAnimationState("Die");
        agent.isStopped = true;
        audioSource.clip = enemyData.deathSound;
        audioSource.Play();
        GetComponent<CapsuleCollider>().enabled = false;
        Material spriteMaterial = spriteRenderer.material;
        LeanTween.value(spriteRenderer.gameObject, 0f, 1f, 1f).setOnUpdate((value) =>
            {
                spriteMaterial.SetFloat("_DissolveAmount", value);
            });
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject, 1f);
    }

    public void ForceDie()
    {
        Die();
    }

    protected void ChangeAnimationState(string newState)
    {
        if (animator == null) return;
        if (currentAnimState == newState) return;
        animator.Play(newState);
        currentAnimState = newState;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(shootTransform.position, aimDirection);
    }
}
