using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider voiceSlider;
    [SerializeField] private Slider textSpeedSlider;

    [Header("Buttons")]
    [SerializeField] private Button confirmSettingsButton;
    [SerializeField] private Button adminButton;

    private const string TextSpeedKey = "TextSpeedMultiplier";
    private const float MinimumVolume = 0.0001f; 

    public static float TextSpeedMultiplier { get; private set; } = 1f;

    private void Start()
    {
        TextSpeedMultiplier = PlayerPrefs.GetFloat(TextSpeedKey, 1f);
        textSpeedSlider.SetValueWithoutNotify(TextSpeedMultiplier);
        
        textSpeedSlider.onValueChanged.AddListener(SetTypingSpeed);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffectsVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
    }

    public void SetSoundEffectsVolume(float value)
    {
        SetMixerVolume("SoundEffectsVolume", value);
    }

    public void SetVoiceVolume(float value)
    {
        SetMixerVolume("VoiceVolume", value);
    }


    private void SetTypingSpeed(float multiplier)
    {
        TextSpeedMultiplier = multiplier;
        PlayerPrefs.SetFloat(TextSpeedKey, multiplier);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        float safeValue = Mathf.Clamp(sliderValue, MinimumVolume, 1f);
        float decibels = Mathf.Log10(safeValue) * 20f;

        audioMixer.SetFloat(parameterName, decibels);
    }

        private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        soundEffectsSlider.onValueChanged.RemoveListener(SetSoundEffectsVolume);
        voiceSlider.onValueChanged.RemoveListener(SetVoiceVolume);
        textSpeedSlider.onValueChanged.RemoveListener(SetTypingSpeed);
    }

}
