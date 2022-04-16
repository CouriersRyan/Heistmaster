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
    
    protected int _layerBoard = (1 << 3) + (1 << 6) + (1 << 7);
    
    private Queue<UnitAction> _queueActions = new Queue<UnitAction>();
    private UnitAction _currentAction;
    [SerializeField] private string currentActionName;
    
    // Start is called before the first frame update
    protected void Start()
    {
        model.agent = GetComponent<NavMeshAgent>();
        _controller = new UnitController(model);
    }
    
    // While there is an action queued, run the action.
    protected void Update()
    {
        if(_currentAction != null &&  !model.knockedOut)_currentAction.Action(_controller, this);
    }

    // Stop the previous action on the queue and run the new action in the front of the queue.
    public virtual UnitAction NextAction()
    {
        // TODO: Object pooling
        if(_currentAction != null) _currentAction.EndAction(_controller, this);
        if (_queueActions.Count > 0)
        {
            _currentAction = _queueActions.Dequeue();
            _currentAction.StartAction(_controller, this);
            currentActionName = _currentAction.actionName;
        }
        else
        {
            _currentAction = null;
        }
        
        return _currentAction;
    }
    
    // Clears all queued actions and immediately runs the new action.
    public void OverwriteActions(UnitAction action)
    {
        _queueActions.Clear();
        _queueActions.Enqueue(action);
        NextAction();
    }
    
    // Creates a new Queue of actions to use as the queue.
    public void OverwriteActionsNewQueue(UnitAction action)
    {
        _queueActions = new Queue<UnitAction>();
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
            
            case UnitActions.Interact:
                newAction = new UnitActionInteract(target);
                break;
            
            default:
            case UnitActions.Idle:
                newAction = new UnitActionIdle(target);
                break;
        }

        return newAction;
    }
    
    // Knocks out or unknocks out the unit, preventing them from taking actions.
    public void SetKnockout(bool knockedOut)
    {
        model.knockedOut = knockedOut;
        OverwriteActions(GetAction(UnitActions.Idle, null));
        foreach (var item in model._items)
        {
            item.SetActive(true);
            item.transform.position = transform.position;
        }
    }
    
    // Sets the speed of the navmesh agent.
    public void SetUnitSpeed(float speed)
    {
        model.agent.speed = speed;
    }
    
    //Adds item to inventory in model
    public void AddItem(GameObject item)
    {
        item.transform.position = Vector3.zero;
        model._items.Add(item);
    }
}
