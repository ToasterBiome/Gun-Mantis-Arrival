using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/New Enemy")]
public class EnemyData : ScriptableObject
{
    public float maxHP;
    public float weaponDamage;

    public AudioClip fireSound;
    public AudioClip deathSound;
}
