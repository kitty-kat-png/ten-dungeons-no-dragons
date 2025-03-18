using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniRat : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float damage = 1f;
    public int health = 5;

    private Transform player;
    private Rigidbody2D rb2d;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb2d.velocity = direction * moveSpeed;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Mini rat destroyed");
        Destroy(gameObject);
    }
}
