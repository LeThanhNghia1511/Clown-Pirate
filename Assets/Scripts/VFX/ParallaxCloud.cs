using UnityEngine;

public class ParallaxCloud : MonoBehaviour
{
    [SerializeField] private float _parallaxEffect = 0.5f;
    [SerializeField] private float _startPosX;
    [SerializeField] Transform _camera;

    private void Start()
    {
        _camera = Camera.main.transform;
        _startPosX = this.transform.position.x;
    }

    private void Update()
    {
        float dist = _camera.position.x * _parallaxEffect;
        transform.position = new Vector3(_startPosX + dist, this.transform.position.y, this.transform.position.z);
    }
}
