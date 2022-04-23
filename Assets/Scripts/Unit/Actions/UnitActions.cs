using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitActions
{
    Idle,
    Move,
    MoveTarget,
    Interact,
    InteractMove
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
        controller.SetStoppingDistance();
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
        // Only updates position to go to as long as target can be seen, otherwise, just go to the last seen position.
        var seeTarget = view.SeeTarget(_target.transform);
        if (seeTarget != null)
        {
            controller.UnitMove(_target.transform.position);
        }
        
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
        if (_interactable == null)
        {
            _interactable = _target.GetComponentInParent<IInteractable>();
        }
        actionName = "Interact";
    }
    
    protected UnitActions unitActions = UnitActions.Move;
    
    // Perform the interact in the interactable object.
    public override void Action(UnitController controller, UnitView view)
    {
        if (_interactable.Interact())
        {
            view.NextAction();
        }
    }

    // Check to see if the player is in range to interact first.
    public override void StartAction(UnitController controller, UnitView view)
    {
        // If not in range of interaction, move to the interaction first and
        // requeue actions so that we check that interaction again.
        if (Vector3.Distance(_target.transform.position, view.transform.position) > _interactable.Range)
        {
            //Requeue.
            var queue = view.GetActions();
            view.OverwriteActionsNewQueue(view.GetAction(unitActions, _target));
            controller.SetStoppingDistance(_interactable.Range);
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
        controller.SetStoppingDistance();
    }
}

// Interact action but for a moving target.
public class UnitActionInteractTarget : UnitActionInteract
{
    public UnitActionInteractTarget(GameObject target) : base(target)
    {
        unitActions = UnitActions.MoveTarget;
    }
}
