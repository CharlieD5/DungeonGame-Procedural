using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // tile that the door is on
    public Tile tile;
    // reference to the connected door
    public Door connectedDoor;
    // to store door direction
    public char direction;
}
