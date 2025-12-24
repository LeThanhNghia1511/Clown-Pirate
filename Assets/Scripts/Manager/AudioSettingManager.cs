using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider _audioSlider;
    private bool _isBGMOn = true;
    private bool _isSFXOn = true;
    private float _currentAudioValue;

    // Icon for toggle button
    [SerializeField] private Image bgmIcon;
    [SerializeField] private Image sfxIcon;
    [SerializeField] private Sprite iconOn;
    [SerializeField] private Sprite iconOff;

    public static AudioSettingsManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadSlider();
    }

    public void SetAudioVolume()
    {
        float audioValue = _audioSlider.value;
        _currentAudioValue = ConvertLinearToDB(audioValue);
        audioMixer.SetFloat("BGMVolume", _currentAudioValue);
        audioMixer.SetFloat("SFXVolume", _currentAudioValue);
        audioMixer.SetFloat("UIVolume", _currentAudioValue);
        PlayerPrefs.SetFloat("BGMVolume", audioValue);
        PlayerPrefs.SetFloat("SFXVolume", audioValue);
        PlayerPrefs.SetFloat("UIVolume", audioValue); // Prefs phai dung gia tri tuyen tinh

        // Update states
        if (_currentAudioValue <= -80f)
        {
            _isBGMOn = false;
            _isSFXOn = false;
        }
        else
        {             
            _isBGMOn = true;
            _isSFXOn = true;
        }
        UpdateMixerAndIcon();
    }

    private float ConvertLinearToDB(float linearVolume)
    {
        if (linearVolume < 0.0001f)
        {
            return -80;
        }
        return Mathf.Log10(linearVolume) * 20;
    }

    public void ToggleBGM()
    {
        if (_isBGMOn)
        {
            audioMixer.SetFloat("BGMVolume", -80);
            _isBGMOn = false;
        }
        else
        {
            audioMixer.SetFloat("BGMVolume", _currentAudioValue);
            _isBGMOn = true;
        }
        UpdateMixerAndIcon();
    }

    public void ToggleSFX()
    {
        if (_isSFXOn)
        {
            audioMixer.SetFloat("SFXVolume", -80);
            _isSFXOn = false;

        }
        else
        {
            audioMixer.SetFloat("SFXVolume", _currentAudioValue);
            _isSFXOn = true;
        }
        UpdateMixerAndIcon();
    }

    private void LoadSlider()
    {
        float linearValue = PlayerPrefs.GetFloat("BGMVolume", 0.7f); // or SFXVolume, whatever
        _audioSlider.value = linearValue;
        SetAudioVolume();
    }

    private void UpdateMixerAndIcon()
    {
        if (bgmIcon != null)
        {
            bgmIcon.sprite = _isBGMOn ? iconOn : iconOff;
        }
        if (sfxIcon != null)
        {
            sfxIcon.sprite = _isSFXOn ? iconOn : iconOff;
        }
    }
}