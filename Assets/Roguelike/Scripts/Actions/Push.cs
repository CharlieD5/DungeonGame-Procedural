using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        if (Input.GetKeyDown(KeyCode.P) && tile.IsAdjacentTo(unit.tile) && unit.mana >= 3)
        {
            unit.mana -= 3;
            gController.currentManaText.text = "Mana: " + unit.mana.ToString() + "/" + unit.maxMana.ToString();

            UnitDir direction;
            
            // Push east
            if (tile.roomPosition.x > unit.tile.roomPosition.x)
            { 
                direction = UnitDir.East;
            }
            // Push west
            else if (tile.roomPosition.x < unit.tile.roomPosition.x)
            {
                direction = UnitDir.West;
            }
            // Push north
            else if (tile.roomPosition.y > unit.tile.roomPosition.y)
            {
                direction = UnitDir.North;
            }
            // Push south
            else
            {
                direction = UnitDir.South;
            }

            otherUnitsPushed(direction, unit, gController, tile);
            return true;
        }
        return false;
    }
    
    void otherUnitsPushed(UnitDir dir, Unit unit, GameController gController, Tile tile)
    {
        Tile pushTo = tile;
        switch (dir)
        {
            case UnitDir.East:
                pushTo = tile.room.GetTileAt(tile.roomPosition + new Vector2Int(1, 0));
                break;
            case UnitDir.West:
                pushTo = tile.room.GetTileAt(tile.roomPosition - new Vector2Int(1, 0));
                break;
            case UnitDir.North:
                pushTo = tile.room.GetTileAt(tile.roomPosition + new Vector2Int(0, 1));
                break;
            case UnitDir.South:
                pushTo = tile.room.GetTileAt(tile.roomPosition - new Vector2Int(0, 1));
                break;
        }

        Unit u = tile.unit;
        Unit otherUnit = pushTo.unit;

        u.wasPushed = true;

        // TODO: enemy can't perform any actions when pushed
        if (pushTo.IsFloorTile)
        {
            gController.MoveUnitToTile(u, pushTo, false);
        }
        else if (pushTo.IsWallTile)
        {
            gController.Damage(u);
        }
        else if (pushTo.IsWaterTile)
        {
            gController.Damage(u);
        }
        else if (pushTo.IsLavaTile) // only works for 1 hp enemies should make death function for more than 1 hp
        {
            gController.MoveUnitToTile(u, pushTo, false);
            gController.Damage(u);
        }

        // Recursive push if another enemy in next tile over
        if (otherUnit != null)
        {
            otherUnitsPushed(dir, otherUnit, gController, pushTo);
        }
    }

}
