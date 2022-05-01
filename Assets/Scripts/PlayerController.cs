using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;

// The class that handles all player inputs and passes it on to other objects in the scene.
// Excluding some menus and UI elements.
public class PlayerController : MonoBehaviour
{

    [SerializeField] private GameObject selected; //Any object the player last clicked on.
    [SerializeField] private UnitView selectedUnit; //The unit the player current has selected.
    [SerializeField] private List<UnitView> listOfUnits;

    // Fields that relate to the camera.
    [SerializeField] private Camera cam;
    [SerializeField] private Transform zoomPos; // Camera position when zoomed in.
    [SerializeField] private Transform overPos; // Camera position when looking at the whole level.
    [SerializeField] private Transform menuPos; // Camera position when inside a menu.
    [SerializeField] private Vector3 offSetPos; // Offset used when focusing in on an object on the board.
    [SerializeField] private float camSpd = 1f; // Speed the camera moves at when zoomed in.

    [SerializeField] private Material[] outlineMaterials; // The material of the units, incharge of outlining.
    [SerializeField] private Transform highlightMarker;

    private Vector2 _mousePos; // Position of mouse in screen space.
    private float _shift; // Holds whether or not the Shift key is being held down.
    private bool _isZoomed; // Is the camera zoomed in.
    private Vector2 _camMove = new Vector2(); // What direction the mouse should move at based on key inputs.
    private Vector2 _camMoveMouse = new Vector2(); // The direction the mouse should move at using edge pan.
    
    private int _layerBoard = (1 << 3) + (1 << 6) + (1 << 7); // A layer mask that only interacts with gameobjects that are part of the board.
    // i.e. not decorations, UI, menus, etc.


    void Start()
    {
        StartCoroutine(ToMenuCamera(false));
        selected = selectedUnit.gameObject;
        _isZoomed = false;
        GameManager.Instance.playerUnits = listOfUnits.Count;
        GameManager.Instance.GameEndEvent += () =>
        {
            StartCoroutine(ToMenuCamera(true));
            selectedUnit = null;
            GameManager.Instance.ChangeScoreScreen(GetTotalPrices());
        };
        GameManager.Instance.GameOverEvent += () =>
        {
            StartCoroutine(ToMenuCamera(true));
            selectedUnit = null;
        };
    }

    // Handles moving the camera.
    private void FixedUpdate()
    {
        // Only moves the zoomed camera
        if (_isZoomed)
        {
            // Create a combined vector for camera movement that prioritizes keyboard inputs over mouse inputs.
            var camMoveCombined = new Vector2();
            camMoveCombined.x = _camMove.x;
            if (camMoveCombined.x == 0) camMoveCombined.x = _camMoveMouse.x;
            camMoveCombined.y = _camMove.y;
            if (camMoveCombined.y == 0) camMoveCombined.y = _camMoveMouse.y;
            
            // Move the camera based on inputs
            var position = cam.transform.position;
            position = new Vector3(position.x + (camMoveCombined.x * camSpd), position.y, position.z + (camMoveCombined.y * camSpd));
            cam.transform.position = position;
        }

        if (highlightMarker != null && selected != null)
        {
            var position = selected.transform.position;
            var transform1 = highlightMarker.transform;
            transform1.position = new Vector3(position.x, transform1.position.y, position.z);
        }
    }

    // Runs when the player right-clicks.
    private void OnMouseRight(InputValue input)
    {
        if (selectedUnit != null)
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
                selected = hit.collider.gameObject;
                selectedUnit = hit.collider.gameObject.GetComponent<UnitView>();
            }
            if (hit.collider.gameObject.CompareTag("Interactable"))
            {
                selected = hit.collider.gameObject;
            }
        }
    }

    // When the mouse position changes, update the variable in the game abject.
    private void OnMousePosition(InputValue input)
    {
        _mousePos = input.Get<Vector2>();
        
        // Update the mouse specific vector for handling moving the camera.
        var newCamMove = new Vector2();
        
        if (_mousePos.x <= 0)
        {
            newCamMove.x = -1;
        } else if (_mousePos.x >= Screen.width - 1)
        {
            newCamMove.x = 1;
        }
        else
        {
            newCamMove.x = 0;
        }
        
        if (_mousePos.y <= 0)
        {
            newCamMove.y = -1;
        } else if (_mousePos.y >= Screen.height - 1)
        {
            newCamMove.y = 1;
        }
        else
        {
            newCamMove.y = 0;
        }

        _camMoveMouse = newCamMove;
    }
    
    // When WASD or Arrow Keys are pressed, updates vector used to move the camera.
    private void OnMoveCamera(InputValue input)
    {
        _camMove = input.Get<Vector2>();
    }

    
    // Toggles between zoomed in camera position and overview camera of the either level.
    private void OnZoom(InputValue input)
    {
        _isZoomed = !_isZoomed;

        if (_isZoomed)
        {
            cam.transform.position = zoomPos.position;
            cam.transform.rotation = zoomPos.rotation;
            foreach (var outlineMaterial in outlineMaterials)
            {
                outlineMaterial.SetFloat("_OutlineThickness", 0.001f);
            }
        }
        else
        {
            var transform1 = cam.transform;
            zoomPos.position = new Vector3(transform1.position.x, zoomPos.position.y, transform1.position.z);
            transform1.position = overPos.position;
            transform1.rotation = overPos.rotation;
            foreach (var outlineMaterial in outlineMaterials)
            {
                outlineMaterial.SetFloat("_OutlineThickness", 0.003f);
            }
        }
    }

    // Toggles between a list of all player units and centers camera on new unit.
    private void OnRotate(InputValue input)
    {
        if (selectedUnit != null)
        {
            int index = listOfUnits.IndexOf(selectedUnit);
            index++;
            if (listOfUnits.Count > index)
            {
                selectedUnit = listOfUnits[index];
            }
            else
            {
                selectedUnit = listOfUnits[0];
            }
        }
        else
        {
            selectedUnit = listOfUnits[0];
        }

        if (_isZoomed)
        {
            cam.transform.position = selectedUnit.transform.position + offSetPos;
        }

        selected = selectedUnit.gameObject;
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
                if (hit.collider.gameObject.CompareTag("Interactable"))
                {
                    action(selectedUnit.GetAction(UnitActions.Interact, hit.collider.gameObject));
                    selected = hit.collider.gameObject;
                    return;
                }
                //TODO: Object pooling
                var target = new GameObject();
                target.AddComponent<StrayActionMarker>();
                selected = target;
                target.transform.position = hit.point;
                action(selectedUnit.GetAction(UnitActions.Move, target));
            }
        }
    }

    // Rotates and moves the camera between its menu position and position for gameplay.
    private IEnumerator ToMenuCamera(bool toMenu)
    {
        Transform target;
        if (toMenu)
        {
            target = menuPos;
        }
        else
        {
            target = overPos;
        }

        var transform1 = cam.transform;
        Vector3 startPos = transform1.position;
        Quaternion startRot = transform1.rotation;

        float t = 0f;

        while (t < 1f)
        {
            cam.transform.position = Vector3.Slerp(startPos, target.position, t);
            cam.transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
            t += Time.deltaTime / 2;
            yield return null;
        }
    }

    // Gets the prices in all player units.
    public int GetTotalPrices()
    {
        int price = 0;
        foreach (var unit in listOfUnits)
        {
            var inventory = unit.Items;
            foreach (var item in inventory)
            {
                Collectible collectible;
                if (item.TryGetComponent(out collectible))
                {
                    price += collectible.Price;
                }
            }
        }

        return price;
    }
}
