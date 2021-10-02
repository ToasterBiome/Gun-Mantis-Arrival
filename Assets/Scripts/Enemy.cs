using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] float currentHP;

    [SerializeField] EnemyData enemyData;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = enemyData.maxHP;
        gameObject.name = enemyData.name;
    }

    // Update is called once per frame
    void Update()
    {

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
