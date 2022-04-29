using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //Singleton implementation.
    private static MainMenuManager _instance;

    public static MainMenuManager Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<MainMenuManager>();
            return _instance;
        }
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }
}
