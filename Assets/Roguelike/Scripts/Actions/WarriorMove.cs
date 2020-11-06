using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMove : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        Tile to;

        // get overall distance between player and unit
        int xDistance = gController.player.tile.roomPosition.x - unit.tile.roomPosition.x;
        int yDistance = gController.player.tile.roomPosition.y - unit.tile.roomPosition.y;

        // If x direction is greater than y direction, go x direction
        if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
        {
            // Move East
            if (xDistance > 0)
            {
                to = unit.tile.room.GetTileAt(unit.tile.roomPosition + new Vector2Int(1, 0));
            }
            // Move West
            else
            {
                to = unit.tile.room.GetTileAt(unit.tile.roomPosition - new Vector2Int(1, 0));
            }
        }
        // If y direction is greater than x direction, go y direction
        else
        {
            // Move North
            if (yDistance > 0)
            {
                to = unit.tile.room.GetTileAt(unit.tile.roomPosition + new Vector2Int(0, 1));
            }
            // Move South
            else
            {
                to = unit.tile.room.GetTileAt(unit.tile.roomPosition - new Vector2Int(0, 1));
            }
        }

        if (gController.CanMoveTo(unit, to))
        {
            gController.MoveUnitToTile(unit, to, false);
            return true;
        }

        return false;
    }
}
