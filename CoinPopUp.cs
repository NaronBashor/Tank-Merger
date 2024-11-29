using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPopUp : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}
