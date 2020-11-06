using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    // list of rooms
    public List<Room> rooms = new List<Room>();

    public Tile GetTileAt(Vector2Int roomPosition)
    {
        foreach (Room room in rooms)
        {
            foreach (Tile tile in room.tiles)
            {
                if (tile.BoardPosition == roomPosition)
                {
                    return tile;
                }
            }
        }
        return null;
    }

    public Vector3 BoardToWorld(Vector2Int boardPosition)
    {
        // TODO: implement
        Vector3 worldPos = new Vector3(boardPosition.x, 0, boardPosition.y) + transform.position;
        return worldPos;
    }

    public Vector2Int WorldToBoard(Vector3 worldPosition)
    {
        Vector3 adj = worldPosition - transform.position;
        Vector2Int boardPosition = new Vector2Int(Mathf.RoundToInt(adj.x), Mathf.RoundToInt(adj.z));
        return boardPosition;
    }
}
