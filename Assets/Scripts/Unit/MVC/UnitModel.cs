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
    public const float StopDist = 0.2f;
}
