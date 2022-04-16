using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    // Do for X amount of time, then either the interactable runs changes and/or you take the item. Return true when
    // task is completed.
    public bool Interact();
}
