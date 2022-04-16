using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private float timer;
    [SerializeField] private float timerMax = 2f;
    private UnitView _interactor;

    private NavMeshObstacle _obstacle;
    [SerializeField] private bool isOpen;
    void Start()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
    }

    IEnumerator ToggleDoor(bool open)
    {
        Quaternion angle;
        float t = 0;
        if (open)
        {
            angle = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            angle = Quaternion.Euler(-90, 0, 0);
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
            if (isOpen)
            {
                StartCoroutine(ToggleDoor(false));
            }
            else
            {
                StartCoroutine(ToggleDoor(true));
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
