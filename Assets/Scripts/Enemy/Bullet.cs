using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 20f;
    //[SerializeField] private float _damage = 10f;

    private Vector2 _direction;
    public void Initialize(Vector2 direction)
    {
        // Khởi tạo hướng và Player
        _direction = direction.normalized;
        this.transform.localScale = new Vector3(direction.x, 1f, 1f); // Dựa vào hướng nvat để điều chỉnh hướng của thanh kiếm
    }

    private void Update()
    {
        StartCoroutine(MoveBullet());
    }

    public IEnumerator MoveBullet()
    {
        // Bay thẳng
        transform.Translate(_direction * _moveSpeed * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
