using System.Data.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    public static Action<float> OnHealthChanged;

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        OnHealthChanged?.Invoke(health);
    }

    public void Heal(float amount, bool allowOverheal)
    {
        health += amount;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (!allowOverheal)
        {
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
        OnHealthChanged?.Invoke(health);
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Die()
    {
        EndPortal.OnEndGame?.Invoke(false);
    }


}
