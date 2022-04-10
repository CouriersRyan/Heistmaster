using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardUnit : UnitView
{
    [SerializeField] private Transform pathContainer;
    private Transform[] _waypoints;

    private bool _isPatrol;

    private void Start()
    {
        base.Start();
        
        _waypoints = new Transform[pathContainer.childCount];

        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypoints[i] = pathContainer.GetChild(i);
        }
        QueuePath();
    }

    public override UnitAction NextAction()
    {
        var action = base.NextAction();
        if(_isPatrol) AppendAction(action);
        return action;
    }
    
    private void QueuePath()
    {
        _isPatrol = _waypoints.Length > 1;
        foreach (var waypoint in _waypoints)
        {
            AppendAction(GetAction(UnitActions.Move, waypoint.gameObject));
        }
    }
    
// For visualizing the path the guard will take.
    private void OnDrawGizmos()
    {
        Vector3 startPos = pathContainer.GetChild(0).position;
        Vector3 prevPos = startPos;
        foreach (Transform waypoint in pathContainer)
        {
            var position = waypoint.position;
            Gizmos.DrawSphere(position, 0.3f);
            Gizmos.DrawLine(prevPos, position);
            prevPos = position;
        }
        Gizmos.DrawLine(prevPos, startPos);
    }
}
