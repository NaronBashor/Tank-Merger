using UnityEngine;

public class ProjectileCleanUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null) {
            if (collision.CompareTag("Projectile")) {
                Destroy(collision.gameObject);
            }
        }
    }
}
