using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Action
{
    public override bool Perform(GameController gController, Unit u, Tile tile = null)
    {
        if (Input.GetMouseButtonDown(1) && gController.CanMoveTo(u, tile))
        {
            Vector2Int dirDifference = tile.roomPosition - u.tile.roomPosition;
            gController.MoveUnitToTile(u, tile);

            if (gController.player.pickup)
            {
                Vector2Int checkAhead = gController.player.tile.roomPosition + dirDifference;

                foreach (Unit unit in tile.room.units)
                {
                    if (unit.tile.roomPosition == checkAhead)
                    {
                        gController.Damage(unit);
                    }
                 } 
            }

            return true;
        }

        return false;
    }
} 
