using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrow : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        if (Input.GetKeyDown(KeyCode.T) && tile.IsFloorTile && gController.player.pickup)
        {
            int xDistance = gController.player.tile.roomPosition.x - tile.roomPosition.x;
            int yDistance = gController.player.tile.roomPosition.y - tile.roomPosition.y;

            if ((xDistance == 0 && Mathf.Abs(yDistance) > 1 && Mathf.Abs(yDistance) <= 3) || (yDistance == 0 && Mathf.Abs(xDistance) > 1 && Mathf.Abs(xDistance) <= 3))
            {
                if (tile.unit)
                {
                    gController.Damage(tile.unit);
                }

                GameObject spear = gController.player.pickup.gameObject;
                tile.pickup = gController.player.pickup;
                gController.player.pickup.transform.parent = null;
                gController.player.pickup = null;
                gController.ThrowSpear(spear.transform, tile);
                return true;
            }
        }
        return false;
    }

}
