
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class SeaShell : BaseEnemy
{
    //[SerializeField] private GameObject _destroyedPrefab;
    //[SerializeField] private Transform _position;

    ////private void Shoot()
    ////{
    ////    _animator.SetTrigger("Shoot");
    ////    GameObject _bullet = Instantiate(_bulletPrefab, this.transform.position, Quaternion.identity); // Trieu hoi vien dan

    ////}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        PlayerHealth _playerHealth = collision.GetComponent<PlayerHealth>();
    //        _animator.SetTrigger("Bite");
    //        //_playerHealth.TakeDamage(_damage, _hitDirection);
    //    }
    //}

    //public override void HandleDeath()
    //{
    //    GameObject debrisObject = Instantiate(_destroyedPrefab, _position.position, Quaternion.identity);
    //    Destroy(this.gameObject);
    //    _animator.SetBool("isDead", true);
    //}
}
