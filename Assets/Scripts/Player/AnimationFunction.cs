using System;
using UnityEngine;

public class AnimationFunction : MonoBehaviour
{
    [SerializeField] private AudioSource step_aiudiosource;

    public void PlayStep()
    {
        if (!step_aiudiosource) return;

        step_aiudiosource.Play();
    }
}
