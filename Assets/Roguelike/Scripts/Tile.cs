using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // keeps track of the type of tile
    public char tileType;

    // local room position
    public Vector2Int roomPosition;

    // reference to the room it is in
    public Room room;
}
