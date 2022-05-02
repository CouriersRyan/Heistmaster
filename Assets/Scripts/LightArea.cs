using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Togglable for lights in an area.
public class LightArea : Togglable
{
    
    // Turns the light area on or off.
    public override void Toggle()
    {
        var area = GetComponent<Collider>();
        area.enabled = !area.enabled;
        var mesh = GetComponent<MeshRenderer>();
        mesh.enabled = !mesh.enabled;
    }
}
