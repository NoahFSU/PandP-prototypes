using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    [Header("Gun Settings")]
    [Range(1, 100)] public int shootDamage;
    [Range(25, 1000)] public int shootDistance;
    [Range(0.1f, 6)] public float shootRate;
    [Range(0.1f, 6)] public float reloadTime;
    [Range(1, 9)] public int projAmmount;
    public int ammoMax;
    public int magAmmount;
    public int magMax;
    public int ammoCurrent;
  public  float headShotMultiplier = 1;
    [Range(0, 10f)] public float inaccuracyDistance;

    [Header("Dependiceies")]
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    public AudioClip emptySound;
    public AudioClip equipSound;
    public AudioClip reloadSound;
    [Range(0, 1)] public float shootVol;
    [Range(0, 1)] public float emptyVol;
    [Range(0, 1)] public float equipVol;
    [Range(0, 1)] public float reloadVol;
    public Image Icon;
}
