using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : MonoBehaviour
{
    // MVC
    [SerializeField] private UnitModel model;
    private UnitController _controller;
    
    
    private Queue<UnitAction> _queueActions = new Queue<UnitAction>();
    private UnitAction _currentAction;
    [SerializeField] private string currentActionName;
    
    // Start is called before the first frame update
    void Start()
    {
        model.agent = GetComponent<NavMeshAgent>();
        _controller = new UnitController(model);
    }
    
    // While there is an action queued, run the action.
    private void Update()
    {
        if(_currentAction != null)_currentAction.Action(_controller, this);
    }

    // Stop the previous action on the queue and run the new action in the front of the queue.
    public UnitAction NextAction()
    {
        // TODO: Object pooling
        if(_currentAction != null) _currentAction.EndAction(_controller, this);
        _currentAction = _queueActions.Dequeue();
        _currentAction.StartAction(_controller, this);
        currentActionName = _currentAction.actionName;
        return _currentAction;
    }
    
    // Clears all queued actions and immediately runs the new action.
    public void OverwriteActions(UnitAction action)
    {
        _queueActions.Clear();
        _queueActions.Enqueue(action);
        NextAction();
    }

    // Adds the action to the queue and immediately runs it if it is the only action.
    public void AppendAction(UnitAction action)
    {
        _queueActions.Enqueue(action);
        if (_currentAction == null)
        {
            NextAction();
        }
    }
    
    // Retrieves the queue of actions.
    public Queue<UnitAction> GetActions()
    {
        return _queueActions;
    }
    
    // Makes an UnitAction object based on enum and assigns the target.
    public UnitAction GetAction(UnitActions action, GameObject target)
    {
        UnitAction newAction;
        switch (action)
        {
            case UnitActions.Move:
                newAction = new UnitActionMove(target);
                break;
            
            default:
            case UnitActions.Idle:
                newAction = new UnitAction(target);
                break;
        }

        return newAction;
    }
}
