using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : UnitController
{
    private ArcherUnit archer;

    private void Start()
    {
        archer = GetComponent<ArcherUnit>();
    }

    public override bool PerformTurn(GameController gController)
    {
        bool tookAction = false;

        foreach (Action action in archer.actions)
        {
            tookAction = action.Perform(gController, archer);
            if (tookAction)
                return true;
        }

        return false;
    }
}
