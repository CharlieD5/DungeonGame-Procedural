using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Pickup pickup;

    // keeps track of the type of tile
    public char tileType;
    
    // reference to the room it is in
    public Room room;

    // local room position
    public Vector2Int roomPosition;

    public Unit unit;
    
    public Vector2Int BoardPosition
    {
        get
        {
            return roomPosition + room.originBoardPosition;
        }
    }

    public bool IsAdjacentTo(Tile tile)
    {
        if (room != tile.room)
            return false;
        Vector2Int delta = roomPosition - tile.roomPosition;
        int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
        return distance == 1;
    }

    public bool IsJumpableTo(Tile tile, PlayerUnit player)
    {
        if (room != tile.room)
            return false;
        Vector2Int delta = roomPosition - tile.roomPosition;
        int distance = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
        return ((distance >= player.jumpMin) && (distance <= player.jumpMax));
    }

    public bool IsLavaTile => tileType == 'L';
    public bool IsVictoryTile = false;
    public bool IsUpgradeTile = false;
    public bool IsFloorTile => tileType == ' ';
    public bool IsWaterTile => tileType == '~';
    public bool IsWallTile => tileType == '-' || tileType == '|';
    public bool IsDoorTile => tileType == 'N' || tileType == 'S' || tileType == 'E' || tileType == 'W';
}
