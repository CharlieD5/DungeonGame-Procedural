using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAttack : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        if (gController.player.tile.IsAdjacentTo(unit.tile))
        {
            gController.Damage(gController.player);
            return true;
        }
        return false;
    }
}
