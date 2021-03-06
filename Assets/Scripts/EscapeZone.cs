using System;
using UnityEngine;

// Area on the board that the player must reach to end the level.
public class EscapeZone : MonoBehaviour
{
    
    // The number of player units inside all Escape Zones in the level.
    private static int _numInZone = 0;

    public static int NumInZone
    {
        get
        {
            return _numInZone;
        }
    }

    private void Awake()
    {
        _numInZone = 0;
    }

    // Whenever a player unit enter an escape zone, increment the count by one and check if all player units have entered.
    // If they have, then end the game.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _numInZone++;
            if (_numInZone == GameManager.Instance.playerUnits)
            {
                GameManager.Instance.GameEnd();
            }
        }
    }

    // Decrements the player unit in zone count whenever a player leaves.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _numInZone--;
        }
    }
}
