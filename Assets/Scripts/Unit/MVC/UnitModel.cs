using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// The Model portion of the Unit MVC
[Serializable]
public class UnitModel
{
    public NavMeshAgent agent;
    
    // Whether or not the unit is knocked out meaning it can't move or receive actions.
    public bool knockedOut;
    
    // Default distance a unit can stop within range in NavMesh.
    public const float StopDist = 0.3f;
    
    // Variables for IInteractable.
    public float timer;
    public float timerMax = 5f;
    public float range;
    
    // Inventory of items the Unit has stored.
    public List<GameObject> items = new List<GameObject>();
}
