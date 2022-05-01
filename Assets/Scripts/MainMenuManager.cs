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

    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject mainMenu;

    // Loads the level of specified index.
    public void LoadLevel(int i)
    {
        SceneManager.LoadScene(i);
    }

    // Toggles between the main menu and level select screens.
    public void LevelSelect()
    {
        levelSelect.SetActive(!levelSelect.activeInHierarchy);
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
    }
}
