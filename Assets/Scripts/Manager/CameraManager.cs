using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private NoiseSettings _horizontalNoise; // Kéo file Noise Ngang vào đây
    [SerializeField] private NoiseSettings _verticalNoise;   // Kéo file Noise Dọc vào đây

    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;
    // Singleton for camera
    public static CameraManager instance { get; private set; }
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
        _shakeTimer = 0;
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        _perlin = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time, int direction = 0)
    {
        if (_perlin == null) return;

        if (direction == 0) _perlin.NoiseProfile = _horizontalNoise; // Default = 0
        else _perlin.NoiseProfile = _verticalNoise;

        _perlin.AmplitudeGain = intensity;
        _shakeTimer = time;
    }

    private void Update()
    {
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0f)
            {
                _perlin.AmplitudeGain = 0f;
            }
        }
    }
}
