using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _spikeDamage = 2.5f;
    [SerializeField] private float _enterDamage = 0.5f;

    private Health _playerHealth = null;
    //[SerializeField] private GameObject _player;
    private Vector2 _hitDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Calculate();

        if (collision.gameObject.CompareTag("Player"))
        {
            _playerHealth = collision.gameObject.GetComponent<Health>();
            Debug.Log("Deal enter damage");
            _playerHealth.TakeDamage(_enterDamage, _hitDirection);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerHealth = null;
        }
    }

    public void DealDamageToPlayer()
    {
        //Calculate(); // Calculate the diretion to create the knockback effect
        if (_playerHealth != null)
        {
            Debug.Log("Deal spike damage");
            _playerHealth.TakeDamage(_spikeDamage, _hitDirection);
        }
        Debug.Log("null health");
    }    

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
