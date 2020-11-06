using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        if (Input.GetKeyDown(KeyCode.Space) && gController.CanJumpTo(unit, tile))
        {
            gController.JumpUnitToTile(unit, tile);
            return true;
        }
        return false;
    }
}
