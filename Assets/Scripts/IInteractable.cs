using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public float Range
    {
        get;
    }

    public bool CanInteract
    {
        get;
    }

    // Do for X amount of time, then either the interactable runs changes and/or you take the item. Return true when
    // task is completed.
    public bool Interact();
    public void StartInteract(UnitView interactor);
    public void EndInteract();
}
