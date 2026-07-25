using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource voiceSource;

    private void Awake()
    {
        // If an AudioManager already exists, destroy this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Preserve this object when another scene loads.
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource == null)
        {
            return;
        }

        if (ambientSource.clip == clip && ambientSource.isPlaying)
        {
            return;
        }

        ambientSource.Stop();
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || SFXSource == null)
        {
            return;
        }

        SFXSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null || voiceSource == null)
        {
            return;
        }

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void StopVoice()
    {
        if (voiceSource != null)
        {
            voiceSource.Stop();
        }
    }
}