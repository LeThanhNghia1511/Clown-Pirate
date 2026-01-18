using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _bgSfxSource;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private int _sfxPoolSize = 5;
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;

    private Dictionary<string, AudioClip> _sfxLibrary = new Dictionary<string, AudioClip>();

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) 
        {
            Destroy(gameObject); 
            return; 
        }
        instance = this;

        for (int i = 0; i < _sfxPoolSize; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = _sfxGroup;
            _sfxPool.Add(newSource);
        }
    }

    public void PlaySFX(string clipName, bool randomizePitch = true)
    {
        if (string.IsNullOrEmpty(clipName)) return;

        if (!_sfxLibrary.ContainsKey(clipName))
        {
            AudioClip clip = Resources.Load<AudioClip>("Audios/" + clipName);
            if (clip == null)
            {
                Debug.LogWarning("Cant find audio: " + clipName);
                return;
            }
            _sfxLibrary.Add(clipName, clip);
        }

        AudioSource sourceToUse = _sfxPool.Find(s => !s.isPlaying);
        if (sourceToUse == null) sourceToUse = _sfxPool[0];

        sourceToUse.pitch = randomizePitch ? Random.Range(_minPitch, _maxPitch) : 1f;
        sourceToUse.PlayOneShot(_sfxLibrary[clipName]);
    }

    public void PlayMusic(string musicName, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audios/" + musicName);
        if (clip != null)
        {
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }
    }

    public void PlayBGSFX(string musicName, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audios/" + musicName);
        if (clip != null)
        {
            _bgSfxSource.clip = clip;
            _bgSfxSource.loop = loop;
            _bgSfxSource.Play();
        }
    }

    public void StopAllMusic()
    {
        if (_bgmSource != null) _bgmSource.Stop();
        if (_bgSfxSource != null) _bgSfxSource.Stop();
        Debug.Log("Stop all BGM.");
    }
}