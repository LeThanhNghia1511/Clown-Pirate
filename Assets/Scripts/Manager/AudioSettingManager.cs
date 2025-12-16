using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider _audioSlider;
    private bool _isBGMOn = true;
    private bool _isSFXOn = true;
    public float currentAudioValue;

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
        currentAudioValue = ConvertLinearToDB(audioValue);
        audioMixer.SetFloat("BGMVolume", currentAudioValue);
        audioMixer.SetFloat("SFXVolume", currentAudioValue);
        PlayerPrefs.SetFloat("BGMVolume", audioValue); // Prefs phai dung gia tri tuyen tinh
        PlayerPrefs.SetFloat("SFXVolume", audioValue);
    }

    private float ConvertLinearToDB(float linearVolume)
    {
        if (linearVolume < 0.0001f)
        {
            return -80;
        }
        return Mathf.Log10(linearVolume) * 20;
    }

    public void OnOffBGM()
    {
        if (_isBGMOn)
        {
            audioMixer.SetFloat("BGMVolume", -80);
            _isBGMOn = false;
        }
        else
        {
            audioMixer.SetFloat("BGMVolume", currentAudioValue);
            _isBGMOn = true;
        }
    }

    public void OnOffSFX()
    {
        if (_isSFXOn)
        {
            audioMixer.SetFloat("SFXVolume", -80);
            _isSFXOn = false;

        }
        else
        {
            audioMixer.SetFloat("SFXVolume", currentAudioValue);
            _isSFXOn = true;
        }
    }

    private void LoadSlider()
    {
        float linearValue = PlayerPrefs.GetFloat("BGMVolume", 0.7f); // or SFXVolume, whatever
        _audioSlider.value = linearValue;
        SetAudioVolume();
    }

}