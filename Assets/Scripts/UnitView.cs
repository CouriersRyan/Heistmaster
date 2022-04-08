using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : MonoBehaviour
{
    [SerializeField] private UnitModel model;
    private UnitController _controller;

    private NavMeshAgent _agent;
    [SerializeField] private Transform target;
    
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _controller = new UnitController(model);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_agent.SetDestination(target.position));
    }
}
