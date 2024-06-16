using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------- Audio Clip --------")]
    public AudioClip menuMusic;
    public AudioClip menuShoot;


    private void Start()
    {
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayThisSoundEffect()
    {
        SFXSource.clip = menuShoot;
        SFXSource.Play();
    }
}
