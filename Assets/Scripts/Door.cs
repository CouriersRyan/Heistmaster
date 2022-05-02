using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

// Door component, opens and closes using IInteractable.
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private float timer;
    [SerializeField] private float timerMax = 2f; // The amount of time is takes to open the door.
    private UnitView _interactor;
    
    [SerializeField] private bool isOpen;


    // Opens or closes the door based on the given boolean.
    IEnumerator ToggleDoor(bool open)
    {
        Quaternion angle;
        float t = 0;
        if (open)
        {
            angle = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            angle = Quaternion.Euler(0, 90, 0);
        }

        var start = transform.rotation;
        var end = transform.rotation * angle;

        while (t < 1)
        {
            transform.rotation = Quaternion.Lerp(start, end, t);
            t += Time.deltaTime;
            yield return null;
        }
    }

    
    
    /*
     * Implementation of the IInteractable component. Allows Units to open and close the door after a certain
     * amount of time has passed while interacting with it.
     */
    [SerializeField] private float range; 
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
            StartCoroutine(ToggleDoor(isOpen));
            isOpen = !isOpen;
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
