using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plays noises at random intervals.
public class RandomIntervalNoise : MonoBehaviour
{
    private AudioSource _audio;
    [SerializeField] private AudioClip sound;

    [SerializeField] private float minTime = 2f;
    [SerializeField] private float maxTime = 5f;
    private float _timer;
    
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        SetRandomTime();
    }

    // Counts down the timer and plays the sound when timer reaches 0 and sets a new time.
    void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            SetRandomTime();
            _audio.PlayOneShot(sound);
        }
    }

    
    // Set a random time for the timer.
    private void SetRandomTime()
    {
        _timer = Random.Range(minTime, maxTime);
    }
}
