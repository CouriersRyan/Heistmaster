using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A guard is a child of the unit class. Is meant to patrol around and try to catch the player.
public class GuardUnit : UnitView
{
    [SerializeField] private Transform pathContainer;
    private Transform[] _waypoints;

    [SerializeField] private Light spotLight;
    [SerializeField] private float viewDistance;
    [SerializeField] private float castRadius = 3f;
    private float _viewAngle;

    private bool _isPatrol;

    private void Start()
    {
        base.Start();

        _viewAngle = spotLight.spotAngle;
        
        //Takes the transforms in the pathContainer and converts it to a path for the guard to patrol.
        _waypoints = new Transform[pathContainer.childCount];

        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypoints[i] = pathContainer.GetChild(i);
        }
        QueuePath();
    }

    private void Update()
    {
        base.Update();
        
        if (CanSeePlayer())
        {
            spotLight.color = Color.red;
        }
        else
        {
            spotLight.color = Color.white;
        }
    }

    // Check if the player is within range to be seen.
    private bool CanSeePlayer()
    {
        var results = Physics.SphereCastAll(transform.position, castRadius, transform.forward, viewDistance, _layerBoard);
        Debug.Log(results.Length);
        foreach (var hit in results)
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!Physics.Linecast(transform.position, hit.transform.position, 1 << 7))
                {
                    return true;
                }
            }
        }

        return false;
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
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
