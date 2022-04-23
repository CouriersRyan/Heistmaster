using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class UnitModel
{
    public NavMeshAgent agent;
    public bool knockedOut;
    public const float StopDist = 0.8f;
    
    public float timer;
    public float timerMax = 5f;
    public float range;

    public List<GameObject> _items = new List<GameObject>();
}
