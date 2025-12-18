using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _enterDamage = 0.5f;
    //[SerializeField] private float _spikeDamage = 2.5f;
    private Vector2 _hitDirection;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Calculate();

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Deal enter damage");
            if (PlayerHealth.instance != null)
            {
                PlayerHealth.instance.TakeDamage(_enterDamage, _hitDirection);
            }
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        _playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
    //        Debug.Log("Deal spike damage");
    //        if (_playerHealth != null)
    //        {
    //            _playerHealth.TakeDamage(_spikeDamage, _hitDirection);
    //        }
    //    }
    //} 

    //private void Calculate()
    //{
    //    if (transform.position.x > _player.transform.position.x)
    //    {
    //        // Kẻ thù (Attacker) đang ở bên phải Player
    //        _hitDirection = Vector2.right;
    //    }
    //    else
    //    {
    //        // Kẻ thù (Attacker) đang ở bên trái Player
    //        _hitDirection = Vector2.left;
    //    }
    //}
}
