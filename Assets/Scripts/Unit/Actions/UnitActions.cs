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
public class UnitActionIdle : UnitAction
{
    private float _wait = 10f;
    public UnitActionIdle(GameObject target) : base(target)
    {
        actionName = "Idle";
    }

    public override void Action(UnitController controller, UnitView view)
    {
        _wait -= Time.deltaTime;
        if (_wait < 0)
        {
            view.NextAction();
        }
    }
}

// Action that moves the unit to a position.
public class UnitActionMove : UnitAction
{
    public UnitActionMove(GameObject target) : base(target)
    {
        actionName = "Move";
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

    public override void EndAction(UnitController controller, UnitView view)
    {
        view.SetStoppingDistance();
    }
}

// Action that moves towards a specific gameobject, constantly updating its position.
public class UnitActionMoveTarget : UnitAction
{
    public UnitActionMoveTarget(GameObject target) : base(target)
    {
        actionName = "MoveTarget";
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
    private IInteractable _interactable;
    public UnitActionInteract(GameObject target) : base(target)
    {
        _interactable = _target.GetComponent<IInteractable>();
        actionName = "Interact";
    }
    
    public override void Action(UnitController controller, UnitView view)
    {
        //take the Interact function from the interactable and perform that.
        if (_interactable.Interact())
        {
            view.NextAction();
        }
    }

    public override void StartAction(UnitController controller, UnitView view)
    {
        //if not in range of interaction, move to the interaction first.
        //requeue actions so that we check that interaction again.
        if (Vector3.Distance(_target.transform.position, view.transform.position) > _interactable.Range)
        {
            //Requeue.
            var queue = view.GetActions();
            view.OverwriteActionsNewQueue(view.GetAction(UnitActions.Move, _target));
            view.SetStoppingDistance(_interactable.Range);
            view.AppendAction(this);
            foreach (var action in queue)
            {
                view.AppendAction(action);
            }
        }
        else
        {
            _interactable.StartInteract(view);
        }
    }

    public override void EndAction(UnitController controller, UnitView view)
    {
        _interactable.EndInteract();
        view.SetStoppingDistance();
    }
}
