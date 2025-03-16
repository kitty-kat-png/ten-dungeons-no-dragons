using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHittable
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

    public int meleeDamage = 1;
    public float meleeDistance = 1f;
    public float meleeRadius = 1f;
    public float meleeCooldown = 1f;
    public float rangedCooldown = 1f;
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    private Vector2 directionVector = Vector2.up;

    public GameObject projectilePrefab;
    public Transform arrowEmitter;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateDirectionVector();
        HandleMeleeAttack();
        HandleRangedAttack();
        HandleDash();

        //Cooldown timers
        if (meleeTimer > 0f) meleeTimer -= Time.deltaTime;
        if (rangedTimer > 0f) rangedTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movementInput = new Vector2(horizontal, vertical);

        Vector2 move = movementInput * moveSpeed * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + move);
    }

    private void UpdateDirectionVector()
    {
        Vector2 inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(inputVec.magnitude > 0)
        {
            directionVector = inputVec.normalized;
        }
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

    /// <summary>
    /// Implementation of IHittable
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(int damage)
    {
        TakeDamage(damage);
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
            arrowEmitter.up = directionVector;
            RangedAttack();
            rangedTimer = rangedCooldown;
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Melee Attack");
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + (directionVector * meleeDistance), meleeRadius); //Melee attack range

        foreach (Collider2D collider in colliders)
        {
            if(collider.transform.TryGetComponent<IHittable>(out IHittable hittable))
            {
                hittable.Hit(meleeDamage);
            }
        }
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged attack");
        GameObject projectile = Instantiate(projectilePrefab, arrowEmitter.position, arrowEmitter.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((transform.position + (Vector3)(directionVector * meleeDistance)), meleeRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(directionVector * 3));
    }
}
