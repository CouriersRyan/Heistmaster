using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

// This class calls DontDestroyOnLoad on this gameobject.
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
    }
}
