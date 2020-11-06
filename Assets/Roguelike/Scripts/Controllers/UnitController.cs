using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController : MonoBehaviour
{
    public abstract bool PerformTurn(GameController gController);
}