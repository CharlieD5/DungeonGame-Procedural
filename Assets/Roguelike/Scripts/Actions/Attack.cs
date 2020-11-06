using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        if (Input.GetKeyDown(KeyCode.A) && tile.unit)
        {
            if (unit.tile.IsAdjacentTo(tile.unit.tile))
            {
                Unit u = tile.unit;
                gController.Damage(u);
                return true;
            }
        }
        return false;
    }
}
