using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] float healAmount;
    [SerializeField] bool allowOverheal;
    protected override void Activate(PlayerManager player)
    {
        player.Heal(healAmount, allowOverheal);
    }
}
