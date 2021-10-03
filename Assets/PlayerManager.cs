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
        SceneManager.LoadScene("GameScene");
    }
}
