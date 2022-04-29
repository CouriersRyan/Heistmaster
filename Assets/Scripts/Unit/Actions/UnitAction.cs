using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FSM
// Class representing an action that the Unit can or will take.
[Serializable]
public class UnitAction
{
    public string actionName = "Action";
    
    protected GameObject _target; //Target for the action.

    public GameObject Target
    {
        get
        {
            return _target;
        }
    }

    public UnitAction(GameObject target)
    {
        _target = target;
    }
    
    // Runs every frame while on this action.
    public virtual void Action(UnitController controller, UnitView view)
    {
    }
    
    // Runs once when this action starts.
    public virtual void StartAction(UnitController controller, UnitView view)
    {
    }
    
    // Runs once when this action ends.
    public virtual void EndAction(UnitController controller, UnitView view)
    {
        //TODO: Pool the target.
    }
}
