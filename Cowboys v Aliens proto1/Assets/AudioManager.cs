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
    public AudioClip lassoThrow;



    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayButtonShoot()
    {
        SFXSource.clip = menuShoot;
        SFXSource.Play();
    }

    public void PlayLassoThrow()
    {
        SFXSource.clip = lassoThrow;
        SFXSource.Play();
    }
}
