using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// A guard is a child of the unit class. Is meant to patrol around and try to catch the player.
public class GuardUnit : UnitView
{
    [SerializeField] private Transform pathContainer;
    private Transform[] _waypoints;

    [SerializeField] private Light spotLight;
    [SerializeField] private Vector2 castRadii;
    
    [SerializeField] private float walkSpd = 2f;
    [SerializeField] private float runSpd = 4.5f;
    
    // Suspicion to become suspicious, suspicion to identify the player unit, max suspicion.
    [SerializeField] private Vector3 suspicionThresholds = new Vector3(4, 10, 20);
    [SerializeField] private float suspicion = 0;
    private float _viewAngle;

    private bool _isPatrol;
    private bool _isChasing;

    private void Start()
    {
        base.Start();

        _viewAngle = spotLight.spotAngle;
        
        //Takes the transforms in the pathContainer and converts it to a path for the guard to patrol.
        _waypoints = new Transform[pathContainer.childCount];

        _castRadius = castRadii.x;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            _waypoints[i] = pathContainer.GetChild(i);
        }
        QueuePath();
    }

    private void Update()
    {
        base.Update();

        var player = SeePlayer();
        
        if (player != null)
        {
            spotLight.color = Color.red;
            suspicion += Time.deltaTime * 2f;
            Debug.Log(suspicion);
            if (suspicion > suspicionThresholds.x && !_isChasing)
            {
                _isPatrol = false;
                _isChasing = true;
                OverwriteActions(GetAction(UnitActions.InteractMove, player.gameObject));
            }
        }
        else
        {
            spotLight.color = Color.white;
            _isChasing = false;
            if (suspicion > 0)
            {
                suspicion -= Time.deltaTime;
            }
            else
            {
                suspicion = 0;
            }
        }
        
        if (suspicion > suspicionThresholds.y)
        {
            SetUnitSpeed(runSpd);
        }
        else
        {
            SetUnitSpeed(walkSpd);
        }
        
        if (suspicion > suspicionThresholds.z)
        {
            suspicion = suspicionThresholds.z;
        }

        if (suspicion < suspicionThresholds.x && !_isPatrol)
        {
            _isPatrol = true;
            QueuePath();
        }
    }

    // Check if the player is within range to be seen.
    public Transform SeePlayer()
    {
        return SeeTarget("Player");
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
        
        Gizmos.DrawSphere(transform.position, _castRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    // Increases the amount the Guard can see when it enters a light area.
    private void OnTriggerEnter(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit); // Accounts for when the light area
                                                                                    // trigger is disabled while guard
                                                                                    // is still in it.

        if (other.CompareTag("Light"))
        {
            _castRadius = castRadii.y;
        }
    }
    
    // Decreases the amount the Guard can see when it exits a light area.
    private void OnTriggerExit(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
        if (other.CompareTag("Light"))
        {
            _castRadius = castRadii.x;
        }
    }
}
