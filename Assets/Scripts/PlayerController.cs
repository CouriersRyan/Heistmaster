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
    
    private int _layerBoard = 1 << 3; // A layer mask that only interacts with gameobjects that are part of the board.
    // i.e. not decorations, UI, menus, etc.

    // Performs actions when the player right-clicks with the mouse on a position on the board.
    private void OnMouseRight(InputValue input)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out hit, 100f, _layerBoard))
        {
            if (selectedUnit != null)
            {
                selectedUnit.GoToPoint(hit.point);
            }
        }
    }
    
    // Performs actions when the player left-clicks with the mouse.
    private void OnMouseLeft(InputValue input)
    {
        
    }

    // When the mouse position changes, update the variable in the game abject.
    private void OnMousePosition(InputValue input)
    {
        _mousePos = input.Get<Vector2>();
    }
}
