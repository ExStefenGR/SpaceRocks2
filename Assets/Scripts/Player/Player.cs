using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private BulletBehavior bulletPrefab;
    [SerializeField] private Transform bulletLaunchOffset;
    [SerializeField] private AudioSource bulletAudioSource;
    [SerializeField] private AudioClip bulletAudioClip;

    private Vector2 currentMovement;
    private Vector2 moveInput;
    private Rigidbody2D rigidBody;
    private Animator animator;

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }

    public IInteractable Interactable { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsPaused)
        {
            ProcessShootingInput();
            ProcessMovementInput();
        }
        UpdateAnimation();
    }

    // Process player input
    private void ProcessMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    // FixedUpdate is called at fixed intervals
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Move the player based on input
    private void MovePlayer()
    {
        currentMovement = moveInput.normalized;
        rigidBody.velocity = currentMovement * movementSpeed;
    }

    // Update animation parameters
    private void UpdateAnimation()
    {
        animator.SetFloat("direction", currentMovement.y);
    }
    private void ProcessShootingInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BulletBehavior bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = bulletLaunchOffset.position;
            bullet.transform.rotation = bulletLaunchOffset.rotation;
            bullet.gameObject.SetActive(true);
            bulletAudioSource.PlayOneShot(bulletAudioClip);
        }
    }
}