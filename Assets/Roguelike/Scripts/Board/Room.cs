using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // position of the origin of the room on the board
    public Vector2Int originBoardPosition;

    // keeps track of the string that created the room
    [TextArea]
    public string strRoom;

    // list of tiles in the room
    public List<Tile> tiles = new List<Tile>();
    // list of the doors in the room
    public List<Door> doors = new List<Door>();

    // list of units in the room
    public List<Unit> units = null;

    // depth level
    public int depth;

    // 
    public Tile GetTileAt(Vector2Int roomPosition)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.roomPosition == roomPosition)
            {
                return tile;
            }
        }
        return null; 
    }
}
