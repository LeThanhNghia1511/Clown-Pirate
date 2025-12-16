using UnityEngine;
using UnityEngine.Audio; // Nếu m dùng AudioMixer

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop;        // Có lặp lại không

    // Dùng để gán AudioMixer Group nếu cần tách BGM/SFX
    // public AudioMixerGroup mixerGroup; 

    [HideInInspector]
    public AudioSource source; // Biến này được gán bằng code trong Awake()
}