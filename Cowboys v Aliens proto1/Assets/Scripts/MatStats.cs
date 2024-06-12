using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MatStats : ScriptableObject
{
    public ParticleSystem hitEffect;
    public AudioClip HitSound;
    public AudioClip WalkSound;
    [Range(0, 1)] public float hitVol;
    [Range(0, 1)] public float WalkVol;
}
