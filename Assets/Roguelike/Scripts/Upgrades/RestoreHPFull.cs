using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreHPFull : Upgrade
{
    public override void PerformUpgrade(PlayerUnit player)
    {
        player.health = player.maxHealth;
    }
}
