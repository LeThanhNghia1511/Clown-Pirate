using System; // Cần cho Array.Find
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // Audio source
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _bgsfxSource;
    [SerializeField] private AudioSource _uiSource;
    [Header("SFX Pooling")]
    [SerializeField] private int _sfxPoolSize = 5; // Số lượng âm thanh có thể phát cùng lúc
    private List<AudioSource> _sfxPool = new List<AudioSource>();
    // --- AUDIO CLIP ---
    // BGM
    [SerializeField] private AudioClip _gameBGM;
    [SerializeField] private AudioClip _mainMenuBGM;
    [SerializeField] private AudioClip _ocean;
    [SerializeField] private AudioClip _winGame;
    [SerializeField] private AudioClip _gameOver;
    // Item
    [SerializeField] private AudioClip _coinClip;
    // Movement & Attack
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _attackClip;
    [SerializeField] private AudioClip _swordImpact;
    [SerializeField] private AudioClip _clamp;
    [SerializeField] private AudioClip _bite;
    // Hit
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _barrelHit;
    [SerializeField] private AudioClip _barrelBreak;
    // Cannon
    [SerializeField] private AudioClip _cannonFire;
    [SerializeField] private AudioClip _cannonExplosion;


    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;


    // Singleton
    public static AudioManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        for (int i = 0; i < _sfxPoolSize; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            _sfxPool.Add(newSource);
        }
    }

    private void PlaySFX(AudioClip clip, bool randomizePitch = true)
    {
        if (clip == null) return;

        AudioSource sourceToUse = _sfxPool.Find(s => !s.isPlaying);

        if (sourceToUse == null) sourceToUse = _sfxPool[0];
        sourceToUse.pitch = randomizePitch ? UnityEngine.Random.Range(_minPitch, _maxPitch) : 1f;
        sourceToUse.PlayOneShot(clip);
    }

    // --- PUBLIC METHODS (Giờ chỉ cần gọi hàm tổng quát) ---

    public void PlayGameBGM()
    {
        _bgmSource.clip = _gameBGM;
        _bgmSource.Play();
        _bgsfxSource.clip = _ocean;
        _bgsfxSource.Play();
    }
    public void PlayMainMenuBGM() { _bgmSource.clip = _mainMenuBGM; _bgmSource.Play(); }
    public void StopGameBGM()
    {
        _bgmSource.Stop();
        _bgsfxSource.Stop();
    }

    // SFX gọi qua hàm PlaySFX để dùng Pool
    public void PlayCoinSFX() => PlaySFX(_coinClip, false);
    public void PlayJumpSFX() => PlaySFX(_jumpClip);
    public void PlayAttackSFX() => PlaySFX(_attackClip);
    public void PlayHitSFX() => PlaySFX(_hitClip);
    public void PlayBarrelHitSFX() => PlaySFX(_barrelHit);
    public void PlayBarrelBreakSFX() => PlaySFX(_barrelBreak);
    public void PlayCannonFireSFX() => PlaySFX(_cannonFire);
    public void PlayCannonExplosionSFX() => PlaySFX(_cannonExplosion);
    public void PlaySwordImpactSFX() => PlaySFX(_swordImpact);
    public void PlayClampSFX() => PlaySFX(_clamp);
    public void PlayBiteSFX() => PlaySFX(_bite);

    // UI thường không cần random pitch để giữ nguyên chất âm
    public void PlayGameWinMusic() => _uiSource.PlayOneShot(_winGame);
    public void PlayGameOverMusic() => _uiSource.PlayOneShot(_gameOver);

    private void OnApplicationQuit()
    {
        Debug.Log("Reset Audio instance");
        instance = null;
    }
}