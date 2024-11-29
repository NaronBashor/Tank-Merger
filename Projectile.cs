using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;

    public int upgradeLevel;

    private void Start()
    {
        projectileDamage = 11 + (upgradeLevel * 2);
    }

    private void Update()
    {
        float screenHeight = Screen.height;
        float baseScreenHeight = 1080f;
        float speedFactor = (Screen.height / baseScreenHeight) * 600;
        transform.Translate(Vector2.up * speedFactor * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) {
            SoundManager.Instance.TriggerSoundEvent("hit");
            collision.GetComponent<Health>().DealDamage(projectileDamage);
            Destroy(this.gameObject);
        }
    }
}