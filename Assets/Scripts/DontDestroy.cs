using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class calls DontDestroyOnLoad on this gameobject.
public class DontDestroy : MonoBehaviour
{
    
    //Singleton implementation.
    private static DontDestroy _instance;

    private static DontDestroy Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType<DontDestroy>();
            return _instance;
        }
    }
    
    private void Awake()
    {
        // If another instance of this singleton is already set, then destroy this current instance.
        if (Instance != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }
}
