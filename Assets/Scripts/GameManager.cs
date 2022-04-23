using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Game Manager that handles the overall game state.
public class GameManager : MonoBehaviour
{
    
    //Singleton implementation.
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    // Game Over is used in cases where the player loses.
    public delegate void GameOverDelegate();
    
    // Game End is used in cases where the player escape. The outcome is not necessarily a win, but can result in one.
    public delegate void GameEndDelegate();

    public event GameOverDelegate GameOverEvent;
    public event GameEndDelegate GameEndEvent;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameEndScreen;

    public int playerUnits;

    private void Awake()
    {
        GameOverEvent = () =>
        {
            gameOverScreen.SetActive(true);
        };

        GameEndEvent = () =>
        {
            gameEndScreen.SetActive(true);
        };
    }

    // Invokes the Game Over event.
    public void GameOver()
    {
        if (GameOverEvent != null) GameOverEvent.Invoke();
    }

    // Invokes the Game End event.
    public void GameEnd()
    {
        if (GameEndEvent != null) GameEndEvent.Invoke();
    }
}
