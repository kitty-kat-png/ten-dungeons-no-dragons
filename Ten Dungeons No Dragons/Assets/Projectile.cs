using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifetime;
    public float sizeMultiplier;

    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private BoxCollider2D collider2d;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private float currentLifetime = 0f;

    private void Start()
    {
        rb2d.velocity = transform.up * speed;
        collider2d.size = collider2d.size * sizeMultiplier;
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if(currentLifetime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent<IHittable>(out IHittable hittable))
        {
            hittable.Hit(damage);
        }

        Destroy(gameObject);
    }
}
