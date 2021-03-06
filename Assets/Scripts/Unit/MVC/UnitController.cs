using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


// The Controller portion of the Unit MVC
public class UnitController
{
    private UnitModel _model;
    
    public UnitController(UnitModel model)
    {
        _model = model;
    }
    
    // Have the unit go to a position.
    public void UnitMove(Vector3 pos)
    {
        _model.agent.SetDestination(pos);
    }

    // Check whether or not the unit is still moving.
    public bool CheckReachedDestination()
    {
        // https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        // Code by Tryz
        if (!_model.agent.pathPending)
        {
            if (_model.agent.remainingDistance <= _model.agent.stoppingDistance)
            {
                if (!_model.agent.hasPath || _model.agent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    return true;
                }
            }
        }

        return false;
    }

    // Sets the maximum stopping distance for the navmesh agent.
    public void SetStoppingDistance(float stopDist = UnitModel.StopDist)
    {
        _model.agent.stoppingDistance = stopDist;
    }
    
}
