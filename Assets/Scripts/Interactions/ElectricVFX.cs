using UnityEngine;

public class ElectricVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparks;

    public void PlaySparks()
    {
        if (sparks == null) return;

        sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        sparks.Play();
    }
}