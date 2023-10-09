using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private Transform bulletLaunchOffset;
    [SerializeField] private AudioSource bulletAudioSource;
    [SerializeField] private AudioClip bulletAudioClip;
    [SerializeField] private BulletBehavior bigBulletPrefab;

    [SerializeField] private float invincibilityDuration = 3.0f;
    [SerializeField] private int maxHealth = 5;

    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashInterval = 0.1f;

    private Coroutine flashCoroutine;
    private SpriteRenderer playerSpriteRenderer;

    private float invincibilityTimer = 0.0f;
    private bool isInvincible = false;
    private int currentHealth;

    [SerializeField] private Slider chargeBar;

    private Vector2 currentMovement;
    private Vector2 moveInput;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private float chargeTime = 0.0f;
    private float chargeThreshold = 1.0f;

    public IInteractable Interactable { get; set; }

    public enum PlayerState
    {
        Normal,
        Paused,
    }

    private PlayerState _currentState;

    public PlayerState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            HandleStateChanged();
        }
    }
    private void HandleStateChanged()
    {
        switch (CurrentState)
        {
            case PlayerState.Normal:
                Time.timeScale = 1f;
                break;
            case PlayerState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        if (CurrentState == PlayerState.Normal)
        {
            // Process mobile input if available, else use keyboard input
            if (Application.isMobilePlatform)
            {
                ProcessMobileInput();
            }
            else
            {
                ProcessChargingAndShootingInput();
                ProcessMovementInput();
            }
        }
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
        UpdateAnimation();
        UpdateChargeBar();
    }
    // Update animation parameters
    private void UpdateAnimation()
    {
        animator.SetFloat("direction", currentMovement.y);
    }

    // FixedUpdate is called at fixed intervals
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Process player input
    private void ProcessMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    // Process mobile input
    private void ProcessMobileInput()
    {
        // Get the mobile controls instance
        MobileControls mobileControls = MobileControls.Instance;
        if (mobileControls == null)
            return;

        moveInput = mobileControls.IsMoving ? mobileControls.MoveInput : Vector2.zero;
        if (mobileControls.IsFiring)
        {
            // Check if the charge time exceeds the charge threshold
            if (chargeTime >= chargeThreshold)
            {
                FireChargedShot();
            }
            else
            {
                FireRegularShot();
            }
            // Reset the charge time
            chargeTime = 0.0f;
            UpdateChargeBar();
        }
    }

    private void ProcessChargingAndShootingInput()
    {
        // Check if the player is holding down the charge/shoot button
        if (Input.GetKey(KeyCode.E))
        {
            // Increment the charge time
            chargeTime += Time.deltaTime;
        }
        // Check if the player releases the charge/shoot button
        else if (Input.GetKeyUp(KeyCode.E))
        {
            // Check if the charge time exceeds the charge threshold
            if (chargeTime >= chargeThreshold)
            {
                FireChargedShot();
            }
            else
            {
                FireRegularShot();
            }
            // Reset the charge time
            chargeTime = 0.0f;
            UpdateChargeBar();
        }
    }
    // Move the player based on input
    private void MovePlayer()
    {
        currentMovement = moveInput.normalized;
        rigidBody.velocity = currentMovement * movementSpeed;
    }

    private void FireChargedShot()
    {
        BulletBehavior bigBullet = Instantiate(bigBulletPrefab, bulletLaunchOffset.position, bulletLaunchOffset.rotation);
        bulletAudioSource.PlayOneShot(bulletAudioClip);
    }
    private void UpdateChargeBar()
    {
        chargeBar.value = Mathf.Clamp01(chargeTime / chargeThreshold);
    }

    private void FireRegularShot()
    {
        BulletBehavior bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = bulletLaunchOffset.position;
        bullet.transform.rotation = bulletLaunchOffset.rotation;
        bullet.gameObject.SetActive(true);
        bulletAudioSource.PlayOneShot(bulletAudioClip);
    }

    private void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashPlayer());
        }
    }

    private IEnumerator FlashPlayer()
    {
        isInvincible = true;
        float timer = flashDuration;

        while (timer > 0)
        {
            playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            timer -= flashInterval;
        }

        playerSpriteRenderer.enabled = true;
        isInvincible = false;
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible)
        {
            TakeDamage(1);
            Debug.Log(currentHealth);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

}