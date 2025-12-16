using UnityEngine;

public class DebrisObject : MonoBehaviour
{
    // Trong script gắn trên GameObject cha (vật thể bị phá hủy)

    [SerializeField] private float _explosionForce = 3f; // Lực đẩy nhẹ ra
    [SerializeField] private float _torqueForce = 30f; // Lực xoay
    private void Awake()
    {
        BreakObject();
    }
    private void BreakObject()
    {
        // Duyệt qua tất cả các mảnh vỡ (Children)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform pieceTransform = transform.GetChild(i);
            Rigidbody2D rb = pieceTransform.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 1. Tách khỏi Parent
                pieceTransform.SetParent(null); // ⭐ Quan trọng: Tách ra khỏi vật thể gốc

                // 2. Bật Vật lý (nếu m đã tắt nó trước đó)
                rb.simulated = true; // Đảm bảo Rigidbody được kích hoạt và bắt đầu rơi

                // 3. Tính toán Hướng Đẩy Ngẫu nhiên
                // Đẩy ra khỏi tâm vật thể gốc (explosionPosition)
                Vector2 direction = (pieceTransform.position - transform.position).normalized;

                // ⭐ 4. Áp dụng Lực Nhẹ
                float randomForce = Random.Range(_explosionForce * 0.8f, _explosionForce * 1.2f);
                rb.AddForce(direction * randomForce, ForceMode2D.Impulse);

                // Xoay ngẫu nhiên
                rb.AddTorque(Random.Range(-_torqueForce, _torqueForce));

                // 5. Tự hủy sau một khoảng thời gian
                Destroy(pieceTransform.gameObject, 3f);
            }
        }

        // Hủy vật thể gốc đã bị phá hủy
        Destroy(gameObject);
    }
}
