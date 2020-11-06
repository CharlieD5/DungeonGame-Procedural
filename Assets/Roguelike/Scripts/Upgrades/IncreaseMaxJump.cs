using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxJump : Upgrade
{
    public override void PerformUpgrade(PlayerUnit player)
    {
        player.jumpMax = player.jumpMax + 1;
    }
}
