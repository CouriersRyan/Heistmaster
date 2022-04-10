using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The class that handles all player inputs and passes it on to other objects in the scene.
// Excluding some menus and UI elements.
// TODO: This class should never be referenced by another class.
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private UnitView selectedUnit; //The unit the player current has selected.

    private Vector2 _mousePos;
    private float _shift;
    
    private int _layerBoard = (1 << 3) + (1 << 6) + (1 << 7); // A layer mask that only interacts with gameobjects that are part of the board.
    // i.e. not decorations, UI, menus, etc.

    
    // Runs when the player right-clicks.
    private void OnMouseRight(InputValue input)
    {
        if (_shift > 0)
        {
            // Performs actions when the player right-clicks with the mouse on a position on the board. End previous action.
            OnMakeAction(selectedUnit.AppendAction);
        }
        else
        {
            // Queues up actions when the player shift+right-clicks with the mouse on a position on the board.
            OnMakeAction(selectedUnit.OverwriteActions);
        }
    }

    // Sets a bool that tells whether the Shift key was pressed or not.
    private void OnShift(InputValue input)
    {
        _shift = input.Get<float>();
    }
    
    // Performs actions when the player left-clicks with the mouse.
    private void OnMouseLeft(InputValue input)
    {
        RaycastHit hit;
        if (CheckRay(out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                selectedUnit = hit.collider.gameObject.GetComponent<UnitView>();
            }
        }
    }

    // When the mouse position changes, update the variable in the game abject.
    private void OnMousePosition(InputValue input)
    {
        _mousePos = input.Get<Vector2>();
    }

    // Shoots out a ray from mouse position and check if it hits something on the board.
    private bool CheckRay(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        return Physics.Raycast(ray, out hit, 100f, _layerBoard);
    }
    
    // Runs when a ray is cast to make an action.
    private void OnMakeAction(Action<UnitAction> action)
    {
        RaycastHit hit;
        if (CheckRay(out hit))
        {
            if (selectedUnit != null)
            {
                //TODO: Object pooling
                var target = new GameObject();
                target.transform.position = hit.point;
                action(selectedUnit.GetAction(UnitActions.Move, target));
            }
        }
    }
}
