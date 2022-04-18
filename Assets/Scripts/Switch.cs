using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Toggles on and off Togglable objects.
public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private float timer;
    [SerializeField] private float timerMax = 2f;
    [SerializeField] private List<Togglable> toggles = new List<Togglable>();
    private UnitView _interactor;

    [SerializeField] private float range = 1f; 
    public float Range
    {
        get { return range; }
    }
    
    private bool _canInteract = true;
    public bool CanInteract
    {
        get
        {
            return _canInteract;
        }
    }
    public bool Interact()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            foreach (var toggle in toggles)
            {
                toggle.Toggle();
            }
            return true;
        }
        return false;
    }

    public void StartInteract(UnitView interactor)
    {
        if (_interactor == null)
        {
            _interactor = interactor;
            _canInteract = false;
            timer = timerMax;
        }
    }

    public void EndInteract()
    {
        _interactor = null;
        _canInteract = true;
    }
}
