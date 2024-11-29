using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float enemyMovementSpeed;

    public bool canMove;

    private void Start()
    {
        canMove = true;
    }

    private void Update()
    {
        if (GetComponent<Health>().currentHealth > 0 && canMove) {
            float baseScreenHeight = 1080f;
            float speedFactor = (Screen.height / baseScreenHeight) * 32;
            transform.Translate(Vector2.down * speedFactor * Time.deltaTime);
        }

        if (this.GetComponent<RectTransform>().localPosition.y < -250) {
            GameObject.Find("EnemyManager").GetComponent<EnemyManager>().OnEnemyDeath();
            Destroy(gameObject);
        }
    }
}