using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon/New Weapon")]
public class Weapon : ScriptableObject
{
    public GameObject model;
    public float shotCooldown;
    public float damage;
    public int maxAmmo;
    public Vector3 muzzleFlashPoint;

    public int shotAmount;
    public float shotSpread;
}
