using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 movement = Vector2.zero;
    private Vector2 move = Vector2.zero;
    private Rigidbody2D rb;
    private Animator anim;
    public bool isPaused = false;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private BulletBehavior BulletPrefab;
    [SerializeField] private Transform LaunchOffset;
    [SerializeField] private AudioSource bulletAudio;
    [SerializeField] private AudioClip bulletClip;
    public IInteractable Interactable { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Animate();
        if (!isPaused)
        {
            ProcessInput();
        }
    }
    private void ProcessInput()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.E))
        {
            _ = Instantiate(BulletPrefab, LaunchOffset.position, LaunchOffset.rotation);
            bulletAudio.PlayOneShot(bulletClip);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        movement = new Vector2(move.x, move.y).normalized;
        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
    }
    private void Animate()
    {
        anim.SetFloat("direction", movement.y);
    }
}
