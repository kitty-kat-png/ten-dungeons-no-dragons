using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using PubSub;

public class PlayerController : MonoBehaviour, IHittable, ISubscriber<UpgradePickedUp>
{
    public GameManager gameManager;

    public int lives = 1;
    public int health = 3;
    public int maxHealth = 3;
    public int damageReduction = 0;
    public float dodgeChance = 0f;
    public float dodgeChancePerStack = 0.15f;
    public int maxSecondWinds = 0;
    private int currentSecondWindsLeft = 0;

    public float moveSpeed = 3f; //Move speed
    public float dashSpeed = 5f; //Dash speed
    [Range(.8f, 10f)]
    public float stepFrequency = 2f;

    public float dashDuration = 0.5f; //How long the dash lasts
    public float dashCooldown = 3f; //Cooldown between dashes
    private float dashTimeLeft = 0f;
    private float dashCooldownTimeLeft = 0f;

    public float rageTime = 8f;
    private float rageTimer = 0f;

    public int meleeDamage = 1;
    /// <summary>
    /// Amount of damage done whle raging. 
    /// This starts at zero because it gets added to for each rage upgrade
    /// in the upgrade inventory so we could do stacking upgrades if we wanted
    /// </summary>
    public int rageMeleeDamage = 0;
    public int rageDamagePerStack = 2;
    public int maxExtraAttacks = 0;
    private int currentExtraAttacksLeft = 0;
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
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnRage;
    public UnityEvent OnSecondWind;
    public UnityEvent OnDodge;
    public UnityEvent OnStep;
    public UnityEvent OnHit;

    // Private

    private Rigidbody2D rb2d;
    private Vector2 movementInput;

    private float stepTimer = 0f;
    private bool moving = false;
    
    private float meleeTimer = 0f;
    private float rangedTimer = 0f;

    private bool raging = false;
    private bool dead;

    private Vector2 directionVector = Vector2.up;

    private List<string> inventory = new List<string>();

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        health = maxHealth;
        OnHealthChanged.Invoke(health);
    }

    private void Start()
    {
        Subscribe();

        InitializeUpgrades();
    }
    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Update()
    {
        if(!dead)
        {
            UpdateDirectionVector();
            HandleMeleeAttack();
            HandleRangedAttack();
            HandleDash();
            HandleRage();
            HandleSecondWind();
            HandleSteps();

            //Cooldown timers
            if (meleeTimer > 0f) meleeTimer -= Time.deltaTime;
            if (rangedTimer > 0f) rangedTimer -= Time.deltaTime;
            stepTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        movementInput = new Vector2(horizontal, vertical);

        if (!dead)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        moving = false;
        Vector2 move = movementInput * moveSpeed * Time.fixedDeltaTime;
        rb2d.MovePosition(rb2d.position + move);
        if (move.magnitude > .1f) moving = true;
    }

    private void HandleSteps()
    {
        if(stepTimer <= 0 && moving)
        {
            stepTimer = 1f / stepFrequency;
            OnStep.Invoke();
        }
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimeLeft <= 0f && UpgradeManager.Instance.GetOwnedUpgrades().Contains(UpgradeType.Dash)) //Left shift to dash
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

    private void HandleRage()
    {
        if(Input.GetKeyDown(KeyCode.F) && UpgradeManager.Instance.GetOwnedUpgrades().Contains(UpgradeType.Rage))
        {
            raging = true;
            rageTimer = rageTime;
            OnRage.Invoke();
        }

        if(raging)
        {
            rageTimer -= Time.deltaTime;
            if(rageTimer < 0)
            {
                raging = false;
            }
        }
    }

    private void HandleSecondWind()
    {
        if(Input.GetKeyDown(KeyCode.Q) && currentSecondWindsLeft > 0)
        {
            health = maxHealth;
            currentSecondWindsLeft -= 1;
            OnHealthChanged.Invoke(health);
            OnSecondWind.Invoke();
        }
    }

    private void Dash()
    {
        Debug.Log("Dashing");
        dashTimeLeft = dashDuration;

        Vector2 dashDirection = movementInput.normalized;
        rb2d.velocity = dashDirection * dashSpeed;

        OnDash.Invoke();
    }

    public void TakeDamage(int damage)
    {
        health -= (damage - (raging ? damageReduction : 0));
        OnHealthChanged.Invoke(health);
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
        float random = Random.value + .01f; // .01f to make it impossible to dodge if dodgeChance is 0
        if(random > dodgeChance && !dead)
        {
            OnHit.Invoke();
            TakeDamage(damage);
        }
        else
        {
            OnDodge.Invoke();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        dead = true;
        PubSub.PubSub.Instance.PostEvent(new PlayerDiedEvent());
        if (health <=0)
        {
            OnDie?.Invoke();
        }
    }

    private void HandleMeleeAttack()
    {
        if (Input.GetButtonDown("Fire1") && (meleeTimer <= 0f || currentExtraAttacksLeft > 0)) //Fire1 is left mouse button
        {
            if(meleeTimer > 0f)
            {
                currentExtraAttacksLeft -= 1;
            }

            MeleeAttack();

            if(meleeTimer <= 0f)
            {
                currentExtraAttacksLeft = maxExtraAttacks;
            }

            meleeTimer = meleeCooldown;
        }
    }

    private void HandleRangedAttack()
    {
        if (Input.GetButtonDown("Fire2") && rangedTimer <= 0f && !raging) //Fire2 is right mouse button
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
                hittable.Hit(raging ? rageMeleeDamage : meleeDamage);
            }
        }

        OnMelee.Invoke();
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged attack");
        GameObject projectile = Instantiate(projectilePrefab, arrowEmitter.position, arrowEmitter.rotation);
        OnRanged.Invoke();
    }

    private void InitializeUpgrades()
    {
        List<UpgradeType> ownedUpgrades = UpgradeManager.Instance.GetOwnedUpgrades();
        for (int i = 0; i < ownedUpgrades.Count; i++)
        {
            switch (ownedUpgrades[i])
            {
                case UpgradeType.Rage:
                    rageMeleeDamage += rageDamagePerStack;
                    break;
                case UpgradeType.ExtraAttack:
                    maxExtraAttacks += 1;
                    currentExtraAttacksLeft = maxExtraAttacks;
                    break;
                case UpgradeType.Evasion:
                    dodgeChance += dodgeChancePerStack;
                    break;
                case UpgradeType.SecondWind:
                    maxSecondWinds += 1;
                    currentSecondWindsLeft = maxSecondWinds;
                    break;
            }
        }
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

    public void Subscribe()
    {
        PubSub.PubSub.Instance.Subscribe<UpgradePickedUp>(this);
    }

    public void Unsubscribe()
    {
        PubSub.PubSub.Instance.Unsubscribe<UpgradePickedUp>(this);
    }

    public void HandleEvent(UpgradePickedUp evt)
    {
        switch (evt.upgradeType)
        {
            case UpgradeType.Rage:
                rageMeleeDamage += rageDamagePerStack;
                break;
            case UpgradeType.ExtraAttack:
                maxExtraAttacks += 1;
                currentExtraAttacksLeft = maxExtraAttacks;
                break;
            case UpgradeType.Evasion:
                dodgeChance += dodgeChancePerStack;
                break;
            case UpgradeType.SecondWind:
                maxSecondWinds += 1;
                currentSecondWindsLeft = maxSecondWinds;
                break;
        }
    }
}
