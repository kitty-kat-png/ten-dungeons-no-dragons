using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IHittable
{
    public GameManager gameManager;

    public int lives;
    public int health;

    public float moveSpeed = 3f; //Move speed
    public float dashSpeed = 5f; //Dash speed

    public float dashDuration = 0.5f; //How long the dash lasts
    public float dashCooldown = 3f; //Cooldown between dashes
    private float dashTimeLeft = 0f;
    private float dashCooldownTimeLeft = 0f;

    public int meleeDamage = 1;
    public float meleeDistance = 1f;
    public float meleeRadius = 1f;
    public float meleeCooldown = 1f;
    public float rangedCooldown = 1f;

    public GameObject projectilePrefab;
    public Transform arrowEmitter;

    // Unity events

    public UnityEvent OnMelee;
    public UnityEvent OnRanged;
    public UnityEvent OnDash;
    public UnityEvent OnDie;

    // Private

    private Rigidbody2D rb2d;
    private Vector2 movementInput;
    
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    private Vector2 directionVector = Vector2.up;

    private List<string> inventory = new List<string>();

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
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
        rb2d.MovePosition(rb2d.position + move);
    }

    private void UpdateDirectionVector()
    {
        Vector2 inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(inputVec.magnitude > 0)
        {
            directionVector = inputVec.normalized;
        }

        arrowEmitter.up = directionVector;
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
            rb2d.velocity = movementInput.normalized * dashSpeed;
        }
    }

    private void Dash()
    {
        Debug.Log("Dashing");
        dashTimeLeft = dashDuration;

        Vector2 dashDirection = movementInput.normalized;
        rb2d.velocity = dashDirection * dashSpeed;
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

    public void AddItemToInventory(string item)
    {
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
            Debug.Log(item + " added to inventory");
        }
    }

    public bool HasItemInInventory(string item)
    {
        return inventory.Contains(item);
    }

    public void DisplayInventory()
    {
        Debug.Log("Inventory: ");
        foreach (var item in inventory)
        {
            Debug.Log(item);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            AddItemToInventory(other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}
