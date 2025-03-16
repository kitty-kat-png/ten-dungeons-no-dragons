using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f; //Move speed
    public float dashSpeed = 5f; //Dash speed

    public float dashDuration = 0.5f; //How long the dash lasts
    public float dashCooldown = 3f; //Cooldown between dashes
    private float dashTimeLeft = 0f;
    private float dashCooldownTimeLeft = 0f;

    private Rigidbody2D rigidbody;
    private Vector2 movementInput;
    //public GameManager gameManager;
    //public SceneManager sceneManager;

    private float horizontal; //Inputs
    private float vertical;

    public int lives;
    public int health;

    public float meleeCooldown = 1f;
    public float rangedCooldown = 1f;
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    public GameObject projectilePrefab;
    public Transform shootArrow;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleMeleeAttack();
        HandleRangedAttack();
        HandleDash();

        //Cooldown timers
        if (meleeTimer > 0f) meleeTimer -= Time.deltaTime;
        if (rangedTimer > 0f) rangedTimer -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movementInput = new Vector2(horizontal, vertical);

        Vector2 move = movementInput * moveSpeed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + move);
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimeLeft <= 0f) //Left shift to dash
        {
            Dash();
            dashCooldownTimeLeft = dashCooldown;
        }

        if (dashTimeLeft > 0)
        {
            dashTimeLeft -= Time.deltaTime;
            rigidbody.velocity = movementInput.normalized * dashSpeed;
        }
    }

    private void Dash()
    {
        Debug.Log("Dashing");
        dashTimeLeft = dashDuration;

        Vector2 dashDirection = movementInput.normalized;
        rigidbody.velocity = dashDirection * dashSpeed;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        //SceneManager.LoadScene("GameOver");
    }

    private void HandleMeleeAttack()
    {
        if (Input.GetButtonDown("Fire1") && meleeTimer <= 0f) //Fire1 is left mouse button
        {
            MeleeAttack();
            meleeTimer = meleeCooldown;
        }
    }

    private void HandleRangedAttack()
    {
        if (Input.GetButtonDown("Fire2") && rangedTimer <= 0f) //Fire2 is right mouse button
        {
            RangedAttack();
            rangedTimer = rangedCooldown;
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Melee Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1f); //Melee attack range

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit " + enemy.name);
            }
        }
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged attack");
        GameObject projectile = Instantiate(projectilePrefab, shootArrow.position, Quaternion.identity);
        Rigidbody2D rigidbodyProjectile = projectile.GetComponent<Rigidbody2D>();
        if (rigidbodyProjectile != null)
        {
            rigidbodyProjectile.velocity = transform.right * 10f; //Projectile speed
        }
    }
}
