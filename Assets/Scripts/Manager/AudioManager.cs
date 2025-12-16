using UnityEngine;
using System; // Cần cho Array.Find

public class AudioManager : MonoBehaviour
{
    // Audio source
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    // Audio Clip
    [SerializeField] private AudioClip _gameBGM;
    [SerializeField] private AudioClip _mainMenuBGM;
    [SerializeField] private AudioClip _coinClip;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _winGame;
    [SerializeField] private AudioClip _gameOver;
    
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
    }

    public void PlayGameBGM()
    {
        _bgmSource.clip = _gameBGM;
        _bgmSource.Play();
    }

    public void PlayMainMenuBGM()
    {
        _bgmSource.clip = _mainMenuBGM;
        _bgmSource.Play();
    }

    public void StopGameBGM()
    {
        _bgmSource.Stop();
    }

    public void PlayCoinSFX()
    {
        _sfxSource.PlayOneShot(_coinClip);
    }

    public void PlayJumpSFX()
    {
        _sfxSource.PlayOneShot(_jumpClip);
    }

    public void PlayGameWinMusic()
    {
        _sfxSource.PlayOneShot(_winGame);
    }

    public void PlayGameOverMusic()
    {
        _sfxSource.PlayOneShot(_gameOver);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Reset Audio instance");
        instance = null;
    }
}