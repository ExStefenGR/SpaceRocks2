using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Player : MonoBehaviour
{
    public readonly float movementSpeed = 5.0f;

    [SerializeField] private Transform bulletLaunchOffset;
    [SerializeField] private AudioSource bulletAudioSource;
    [SerializeField] private AudioClip bulletAudioClip;

    private readonly float invincibilityDuration = 10.0f;
    private readonly float regularInvincibilityDuration = 3.0f;
    private readonly int maxHealth = 5;

    private readonly float flashDuration = 3.0f;
    private readonly float flashInterval = 0.1f;

    private Coroutine flashCoroutine;
    private SpriteRenderer playerSpriteRenderer;

    private float invincibilityTimer = 0.0f;
    private bool isInvincible = false;
    private bool isTemporaryInvincible = false;

    private int currentHealth;

    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private Slider chargeBar;

    private Vector2 currentMovement;
    private Vector2 moveInput;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private float chargeTime = 0.0f;
    private readonly float chargeThreshold = 1.0f;

    private bool isDiagonalShootingActive = false;


    public IInteractable Interactable { get; set; }
    private ParticleSystem invincibilityParticleSystem;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        invincibilityParticleSystem = GetComponent<ParticleSystem>();
        currentHealth = maxHealth;
        ChangeHealthUI(currentHealth);
    }

    private void Update()
    {
        if (currentHealth != 0)
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
        UpdateAnimation();
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
        if (Input.GetKey(KeyCode.E))
        {
            chargeTime += Time.deltaTime;
            UpdateChargeBar();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
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
    private void MovePlayer()
    {
        currentMovement = moveInput.normalized;
        rigidBody.velocity = currentMovement * movementSpeed;
    }

    private void FireChargedShot()
    {
        BulletBehavior bigBullet = BulletPool.Instance.GetBigBullet();
        bigBullet.transform.position = bulletLaunchOffset.position;
        bigBullet.transform.rotation = bulletLaunchOffset.rotation;
        bigBullet.gameObject.SetActive(true);
        bulletAudioSource.PlayOneShot(bulletAudioClip);
    }

    private void UpdateChargeBar()
    {
        chargeBar.value = Mathf.Clamp01(chargeTime / chargeThreshold);
    }

    private void FireRegularShot()
    {
        BulletBehavior regularBullet = BulletPool.Instance.GetRegularBullet();
        regularBullet.transform.SetPositionAndRotation(bulletLaunchOffset.position, bulletLaunchOffset.rotation);
        regularBullet.gameObject.SetActive(true);
        bulletAudioSource.PlayOneShot(bulletAudioClip);

        // If diagonal shooting is active
        if (isDiagonalShootingActive)
        {
            // Shoot up-diagonal
            BulletBehavior upDiagonalBullet = BulletPool.Instance.GetRegularBullet();
            upDiagonalBullet.transform.SetPositionAndRotation(bulletLaunchOffset.position, bulletLaunchOffset.rotation * Quaternion.Euler(0, 0, 45));
            upDiagonalBullet.gameObject.SetActive(true);

            // Shoot down-diagonal
            BulletBehavior downDiagonalBullet = BulletPool.Instance.GetRegularBullet();
            downDiagonalBullet.transform.SetPositionAndRotation(bulletLaunchOffset.position, bulletLaunchOffset.rotation * Quaternion.Euler(0, 0, -45));
            downDiagonalBullet.gameObject.SetActive(true);
        }
    }

    private void TakeDamage(int damageAmount)
    {
        if (isInvincible || isTemporaryInvincible) return;

        currentHealth -= damageAmount;
        ChangeHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath();
        }
        else
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashPlayer());
            StartCoroutine(TemporaryInvincibility());
        }
    }

    private IEnumerator TemporaryInvincibility()
    {
        isTemporaryInvincible = true;

        float duration = isTemporaryInvincible && isInvincible ? invincibilityDuration : regularInvincibilityDuration;

        yield return new WaitForSeconds(duration);

        isTemporaryInvincible = false;
    }

    private void HPUp(int hpUp)
    {
        currentHealth += hpUp;
        ChangeHealthUI(currentHealth);
    }

    private IEnumerator FlashPlayer()
    {
        float timer = flashDuration;
        Color originalColor = playerSpriteRenderer.color;

        while (timer > 0)
        {
            playerSpriteRenderer.color = (playerSpriteRenderer.color == Color.clear) ? originalColor : Color.clear;
            yield return new WaitForSeconds(flashInterval);
            timer -= flashInterval;
        }

        playerSpriteRenderer.color = originalColor;
    }


    private void OnDeath()
    {
        chargeTime = 0.0f;
        UpdateChargeBar();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible && !isTemporaryInvincible && !collision.gameObject.CompareTag("barrier"))
        {
            TakeDamage(1);
            isTemporaryInvincible = true;
        }
    }


    private void ChangeHealthUI(int health)
    {
        hp.text = $"HP: {health}";
    }

    public void ActivatePowerUp(PowerUpType type)
    {
        StartCoroutine(DeactivatePowerup(type));
        switch (type)
        {
            case PowerUpType.DiagonalShooting:
                isDiagonalShootingActive = true;
                break;
            case PowerUpType.Forcefield:
                isInvincible = true;
                invincibilityParticleSystem.Play();
                break;
            case PowerUpType.LifeUp:
                HPUp(1);
                break;
        }

        StartCoroutine(DeactivatePowerup(type));
    }


    private IEnumerator DeactivatePowerup(PowerUpType type)
    {
        float duration = 0;

        switch (type)
        {
            case PowerUpType.DiagonalShooting:
                duration = 20.0f;
                break;
            case PowerUpType.Forcefield:
                duration = invincibilityDuration;
                break;
        }

        yield return new WaitForSeconds(duration);

        switch (type)
        {
            case PowerUpType.DiagonalShooting:
                isDiagonalShootingActive = false;
                break;
            case PowerUpType.Forcefield:
                invincibilityParticleSystem.Stop();
                isInvincible = false;
                break;
        }
    }

    private IEnumerator ForceField()
    {
        isInvincible = true;
        invincibilityParticleSystem.Play();

        while (invincibilityTimer > 0)
        {
            yield return new WaitForSeconds(1);
            invincibilityTimer -= 1;
        }

        invincibilityParticleSystem.Stop();
        isInvincible = false;
    }

    public void ActivateForceField()
    {
        invincibilityTimer = invincibilityDuration;
        StartCoroutine(ForceField());
    }
}