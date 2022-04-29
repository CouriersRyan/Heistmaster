using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

// Class that is used when we need to create Transforms to act as targets for actions rather than existing transforms.
public class StrayActionMarker : MonoBehaviour
{
    private float _timer = 0.5f;

    private bool _isUsing = true;

    
    // Sets whether or not this object is being used. If it isn't, then start a coroutine to destroy/pool it, if it is
    // not used again after a certain amount of time.
    public void SetUsing(bool isUsing)
    {
        _isUsing = isUsing;

        if (!_isUsing)
        {
            StartCoroutine(TimePool());
        }
        else
        {
            StopCoroutine(TimePool());
        }
    }

    
    // After 0.5 seconds, destroy this object, unless it becomes in use again.
    private IEnumerator TimePool()
    {
        _timer = 0.5f;

        while (!_isUsing)
        {
            _timer -= Time.deltaTime;

            if (_timer < 0)
            {
                Destroy(this);
            }

            yield return null;
        }
    }
}
