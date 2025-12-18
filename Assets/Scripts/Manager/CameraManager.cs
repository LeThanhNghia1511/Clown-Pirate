using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
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
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        _shakeTimer = time;
    }

    private void Update()
    {
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0f)
            {
                // Time over
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
            }
        }
    }
}
