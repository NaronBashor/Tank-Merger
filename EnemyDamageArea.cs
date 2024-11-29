using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamageArea : MonoBehaviour
{
    Collider2D coll;

    public UIManager manager;

    public int enemyCount;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        enemyCount = 0;
    }

    private void Update()
    {
        if (enemyCount == 0) { return; }

        if (enemyCount > 0) {
            manager.GetComponent<UIManager>().ApplyDamage(enemyCount * 0.0625f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null) {
            if (collision.CompareTag("Enemy")) {
                collision.GetComponent<Enemy>().canMove = false;
                AddEnemy();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null) {
            if (collision.CompareTag("Enemy")) {
                RemoveEnemy();
            }
        }
    }

    public void RemoveEnemy(int amount = 1)
    {
        enemyCount -= amount;
    }

    public void AddEnemy(int amount = 1)
    {
        enemyCount += amount;
    }
}