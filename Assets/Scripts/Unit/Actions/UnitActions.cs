using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActions
{
    Idle,
    Move,
    Interact
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
            view.NextAction();
        }
    }

    public override void StartAction(UnitController controller, UnitView view)
    {
        controller.UnitMove(_target.transform.position);
    }
}

// Action that moves towards a specific gameobject, constantly updating its position.
public class UnitActionMoveTarget : UnitAction
{
    public UnitActionMoveTarget(GameObject target) : base(target)
    {
    }

    public override void Action(UnitController controller, UnitView view)
    {
        controller.UnitMove(_target.transform.position);
        if (controller.CheckReachedDestination())
        {
            view.NextAction();
        }
    }
}

// Interact with an object and change behavior based on the interaction set on that object.
public class UnitActionInteract : UnitAction
{
    public UnitActionInteract(GameObject target) : base(target)
    {
    }
    
    public override void Action(UnitController controller, UnitView view)
    {
        //take the Interact function from the interactable and perform that.
        var interactable = _target.GetComponent<IInteractable>();
        interactable.Interact();
    }

    public override void StartAction(UnitController controller, UnitView view)
    {
        //if not in range of interaction, move to the interaction first.
        //requeue actions so that we check that interaction again.
    }
}
