using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActions
{
    Idle,
    Move
}

/*
 * The following is a series of classes that are children of the UnitAction class.
 * These classes were made so that we queue actions onto a unit.
 */


// Action that moves the unit to a position.
public class UnitActionMove : UnitAction
{
    public UnitActionMove(GameObject target) : base(target)
    {
    }

    public override void Action(UnitController controller, UnitView view)
    {
        if (controller.CheckReachedDestination())
        {
            Debug.Log("reached");
            view.NextAction();
        }
    }

    public override void StartAction(UnitController controller, UnitView view)
    {
        controller.UnitMove(_target.transform.position);
    }
}
