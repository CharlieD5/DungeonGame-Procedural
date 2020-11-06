using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public abstract bool Perform(GameController gController, Unit unit, Tile tile = null);
}
