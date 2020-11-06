using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : UnitController
{
    private WarriorUnit warrior;

    private void Start()
    {
        warrior = GetComponent<WarriorUnit>();
    }

    public override bool PerformTurn(GameController gController)
    {
        bool tookAction = false;

        foreach (Action action in warrior.actions)
        {
            tookAction = action.Perform(gController, warrior);
            if (tookAction)
                return true;
        }

        return false;
    }
}
