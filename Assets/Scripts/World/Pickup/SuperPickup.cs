using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPickup : Pickup
{
    protected override void Activate(PlayerManager player)
    {
        player.GetComponent<GunController>().ActivateSuper();
    }
}
