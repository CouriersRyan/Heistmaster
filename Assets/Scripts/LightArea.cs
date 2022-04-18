using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Togglable for lights in an area.
public class LightArea : Togglable
{
    public override void Toggle()
    {
        var area = GetComponent<BoxCollider>();
        area.enabled = !area.enabled;
    }
}
