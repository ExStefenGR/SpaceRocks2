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

    [SerializeField] private readonly float invincibilityDuration = 3.0f;
    [SerializeField] private readonly int maxHealth = 5;

    private readonly float flashDuration = 3f;
    private readonly float flashInterval = 0.1f;

    private Coroutine flashCoroutine;
    private SpriteRenderer playerSpriteRenderer;

    private float invincibilityTimer = 0.0f;
    private bool isInvincible = false;
    private int currentHealth;

    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private Slider chargeBar;

    private Vector2 currentMovement;
    private Vector2 moveInput;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private float chargeTime = 0.0f;
    private readonly float chargeThreshold = 1.0f;


    public IInteractable Interactable { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
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

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
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
        regularBullet.transform.position = bulletLaunchOffset.position;
        regularBullet.transform.rotation = bulletLaunchOffset.rotation;
        regularBullet.gameObject.SetActive(true);
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
        chargeTime = 0.0f;
        UpdateChargeBar();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible && !collision.gameObject.CompareTag("barrier"))
        {
            TakeDamage(1);
            ChangeHealthUI(currentHealth);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    private string ChangeHealthUI(int health)
    {
        string result = $"HP: {health}";
        return hp.text = result;
    }

}