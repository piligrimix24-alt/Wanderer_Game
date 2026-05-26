using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    //[Header("Volume Settings")]
    //[Range(0f, 1f)]
    //[SerializeField] private float musicVolume = 0.5f;
    //[Range(0f, 1f)]
    //[SerializeField] private float sfxVolume = 0.7f;

    private void Awake()
    {
        // Синглтон паттерн
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }
}