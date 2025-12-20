using UnityEngine;

public class DebrisObject : MonoBehaviour
{
    // Trong script gắn trên GameObject cha (vật thể bị phá hủy)

    [SerializeField] private float _explosionForce = 20f; // Lực đẩy nhẹ ra
    [SerializeField] private float _torqueForce = 30f; // Lực xoay
    private void Awake()
    {
        Explosion();
    }
    private void Explosion()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform pieceTransform = transform.GetChild(i);
            Rigidbody2D rb = pieceTransform.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                pieceTransform.SetParent(null); // ⭐ Quan trọng: Tách ra khỏi vật thể gốc
                rb.simulated = true; // Đảm bảo Rigidbody được kích hoạt và bắt đầu rơi
                // Đẩy ra khỏi tâm vật thể gốc (explosionPosition)
                Vector2 direction = (pieceTransform.position - transform.position).normalized;
                if (direction.y < 0f)
                {
                    direction.x = direction.x > 0f ? 1f : -1f;
                }
                float randomForce = Random.Range(_explosionForce * 0.8f, _explosionForce * 1.2f);
                rb.AddForce(direction * randomForce, ForceMode2D.Impulse);

                // Xoay ngẫu nhiên
                rb.AddTorque(Random.Range(-_torqueForce, _torqueForce));

                // Tự hủy sau một khoảng thời gian
                Destroy(pieceTransform.gameObject, 2f);
            }
        }
        // Hủy vật thể gốc đã bị phá hủy
        Destroy(gameObject);
    }
}
