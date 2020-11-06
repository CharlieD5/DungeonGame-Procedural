using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxMana : Upgrade
{
    public override void PerformUpgrade(PlayerUnit player)
    {
        player.maxMana = player.maxMana + 1;
    }
}
