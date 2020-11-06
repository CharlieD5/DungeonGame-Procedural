using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHP : Upgrade
{
    public override void PerformUpgrade(PlayerUnit player)
    {
        player.maxHealth = player.maxHealth + 1;
    }
}
