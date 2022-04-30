using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : MonoBehaviour, IInteractable
{
    // MVC
    [SerializeField] private UnitModel model;
    private UnitController _controller;
    
    protected int _layerBoard = (1 << 3) + (1 << 6) + (1 << 7);
    
    private Queue<UnitAction> _queueActions = new Queue<UnitAction>();
    private UnitAction _currentAction;
    [SerializeField] private string currentActionName;
    
    [SerializeField] protected float viewDistance;
    protected float _castRadius = 5f;

    public List<GameObject> Items
    {
        get
        {
            return model.items;
        }
    }
    
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
        if(model.knockedOut) return null;
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
            
            case UnitActions.MoveTarget:
                newAction = new UnitActionMoveTarget(target);
                break;
            
            case UnitActions.Interact:
                newAction = new UnitActionInteract(target);
                break;
            
            case UnitActions.InteractMove:
                newAction = new UnitActionInteractTarget(target);
                break;
            
            default:
            case UnitActions.Idle:
                newAction = new UnitActionIdle(target);
                break;
        }

        return newAction;
    }
    
    // Knocks out or unknocks out the unit, preventing them from taking actions.
    // If it is any player unit that gets knocked out, end the game.
    public void SetKnockout(bool knockedOut)
    {
        model.knockedOut = knockedOut;
        OverwriteActions(GetAction(UnitActions.Idle, null));
        foreach (var item in model.items)
        {
            item.SetActive(true);
            item.transform.position = transform.position;
        }
        if(gameObject.CompareTag("Player")) GameManager.Instance.GameOver();
    }

    public bool GetKnockout()
    {
        return model.knockedOut;
    }

    // Check if the target is within range to be seen.
    // For checking for a specific transform.
    public Transform SeeTarget(Transform target)
    {
        Func<Transform, bool> check = t =>
        {
            return t == target;
        };
        return SeeTarget(check);
    }
    
    // Overloaded method, checks for specific tag.
    public Transform SeeTarget(string targetTag)
    {
        Func<Transform, bool> check = t =>
        {
            return t.CompareTag(targetTag);
        };
        return SeeTarget(check);
    }
    
    // main bulk of the overloaded SeeTarget method. Takes in an anonymous method and uses that are the comparison.
    private Transform SeeTarget(Func<Transform, bool> check)
    {
        var results = Physics.SphereCastAll(transform.position, _castRadius, transform.forward, viewDistance, _layerBoard);
        foreach (var hit in results)
        {
            if (check(hit.collider.transform))
            {
                
                if (!Physics.Linecast(transform.position, hit.transform.position, (1 << 3) + (1 << 7)))
                {
                    Debug.Log("hit");
                    return hit.transform;
                }
            }
        }

        return null;
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
        model.items.Add(item);
    }

    
    
    /*
     *  Implementation for interactable on a unit.
     *  Interaction is for knocking out and restoring units.
     */
    
    private UnitView _interactor;
    private bool _canInteract;
    public float Range
    {
        get { return model.range; }
    }
    public bool CanInteract
    {
        get { return _canInteract; }
    }
    public bool Interact()
    {
        if (Vector3.Distance(_interactor.transform.position, transform.position) > Range)
        {
            return true;
        }
        
        if (model.knockedOut)
        {
            model.timer -= Time.deltaTime;
            if (model.timer < 0)
            {
                SetKnockout(false);
                return true;
            }

            return false;
        }
        else
        {
            SetKnockout(true);
            return true;
        }
    }

    public void StartInteract(UnitView interactor)
    {
        if (_interactor == null)
        {
            _interactor = interactor;
            _canInteract = false;
            model.timer = model.timerMax;
        }
    }

    public void EndInteract()
    {
        _interactor = null;
        _canInteract = true;
    }
}
