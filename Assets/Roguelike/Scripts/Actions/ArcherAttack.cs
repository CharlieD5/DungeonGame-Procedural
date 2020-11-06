using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttack : Action
{
    public override bool Perform(GameController gController, Unit unit, Tile tile = null)
    {
        bool isInLineOfSight = false;

        // get overall distance between player and unit
        int xDistance = gController.player.tile.roomPosition.x - unit.tile.roomPosition.x;
        int yDistance = gController.player.tile.roomPosition.y - unit.tile.roomPosition.y;

        Tile checkTile = unit.tile;
        Vector2Int originalTile = unit.tile.roomPosition;

        Tile currentTile = checkTile;

        // Is player in view of archer
        if (xDistance == 0 && Mathf.Abs(yDistance) > 1) // same x axis
        {
            
            while (currentTile != gController.player.tile)
            {
                if (yDistance > 0) // archer south player north, so check LOS in positive direction
                {
                    // if wall return false
                    if (currentTile.IsWallTile)
                        return false;

                    currentTile = currentTile.room.GetTileAt(currentTile.roomPosition + new Vector2Int(0, 1));
                }
                else // player south archer north, so check LOS in negative direction
                {
                    // if wall return false
                    if (currentTile.IsWallTile)
                        return false;

                    currentTile = currentTile.room.GetTileAt(currentTile.roomPosition - new Vector2Int(0, 1));
                }
            }

            isInLineOfSight = true;
        }
        else if (yDistance == 0 && Mathf.Abs(xDistance) > 1) // same y axis
        {
            while (currentTile != gController.player.tile)
            {
                if (xDistance > 0) // archer on left player on right, so check LOS in positive direction
                {
                    // if wall return false
                    if (currentTile.IsWallTile)
                        return false;

                    currentTile = currentTile.room.GetTileAt(currentTile.roomPosition + new Vector2Int(1, 0));
                }
                else // player on left archer on right, so check LOS in negative direction
                {
                    // if wall return false
                    if (currentTile.IsWallTile)
                        return false;

                    currentTile = currentTile.room.GetTileAt(currentTile.roomPosition - new Vector2Int(1, 0));
                }
            }

            isInLineOfSight = true;
        }

        unit.tile.roomPosition = originalTile;

        // shoots arrow at player if in line of sight
        if (isInLineOfSight)
        {
            gController.spawnAndShootArrow(unit.tile);
            gController.Damage(gController.player);
            return true;
        }

        // if not return false
        return false;
    }
}

