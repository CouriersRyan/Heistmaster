using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for collectible objects with a value and interaction.
public class Collectible : MonoBehaviour, IInteractable
{

    [SerializeField] private float timer;
    [SerializeField] private float timerMax = 2f;

    // Price value of the item.
    [SerializeField] private int price;
    public int Price
    {
        get
        {
            return price;
        }
    }
    
    // The Unit that is doing the interacting.
    private UnitView _interactor;

    
    // Maximum distance the interactor can be to interact with this.
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
            _interactor.AddItem(gameObject);
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
