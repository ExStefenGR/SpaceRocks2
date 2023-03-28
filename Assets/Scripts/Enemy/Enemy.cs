using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Vector2 movement = Vector2.zero;
    private Rigidbody2D rb;
    private Animator anim;
    public bool isPaused = false;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform launchOffset;
    [SerializeField] private AudioSource bulletAudio;
    [SerializeField] private AudioClip bulletClip;
    // Enemy Parameters
    [SerializeField] private int hp = 1;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float followThreshold = 5.0f;
    // Handlers
    [SerializeField] private Slider hpSlider;
    private GameObject target;
    private Transform targetTransform;
    private EnemyState randomState;
    private bool hasStoppedFollowing = false;

    // Enum for states
    private enum EnemyState
    {
        MoveInLine,
        Follow,
        FireInLine,
        StopFollow
    };

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        randomState = (EnemyState)Random.Range(0, 4); // Increase the range to include a new state
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        target = GameObject.Find("Player");
        targetTransform = target.transform;

        hpSlider.maxValue = hp;
        hpSlider.value = hp;

        if (randomState is EnemyState.FireInLine)
        {
            InvokeRepeating(nameof(Shoot), 0, Random.Range(1,fireRate));
        }
    }

    // FixedUpdate is called once per physics frame
    private void FixedUpdate()
    {
        if (!isPaused)
        {
            ProcessInput();
            Move();
            Animate();
        }
    }

    // Behaviour here
    private void ProcessInput()
    {
        switch (randomState)
        {
            case EnemyState.MoveInLine:
                MoveInLine();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.FireInLine:
                FireInLine();
                break;
            case EnemyState.StopFollow:
                StopFollow();
                break;
        }
    }

    private void Move()
    {
        Vector2 targetVelocity = new(movement.x * speed, movement.y * speed);
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime);
    }

    private void Animate()
    {
        anim.SetFloat("direction", movement.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag("bullet"))
        {
            hp--;
            hpSlider.value = hp;
            if (hp <= 0)
            {
                ScoreManager.Instance.AddScore(125);
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Enemybarrier"))
        {
            Destroy(gameObject);
        }
    }

    private void Shoot()
    {
        _ = Instantiate(bulletPrefab, launchOffset.position, launchOffset.rotation);
        bulletAudio.PlayOneShot(bulletClip);
    }

    private void MoveInLine()
    {
        movement.x = -1.0f;
        movement.y = 0.0f;
    }

    private void Follow()
    {
        Vector2 targetOffset = (Vector2)targetTransform.position - rb.position;
        float distance = targetOffset.magnitude;
        float slowingRadius = 0.5f; // Adjust this value to control the slowing down distance
        float targetSpeed = (distance > slowingRadius) ? speed : speed * (distance / slowingRadius);
        Vector2 desiredVelocity = targetOffset.normalized * targetSpeed;
        movement = desiredVelocity - rb.velocity;
    }

    private void FireInLine()
    {
        movement.x = -1.0f;
        movement.y = 0.0f;
    }

    private void StopFollow()
    {
        if (!hasStoppedFollowing)
        {
            if (Vector2.Distance(transform.position, targetTransform.position) < followThreshold) // Set the distance threshold
            {
                movement.x = -1.0f;
                movement.y = 0.0f;
                hasStoppedFollowing = true; // Set the flag to true once the enemy stops following
            }
            else
            {
                Follow();
            }
        }
        else
        {
            movement.x = -1.0f;
            movement.y = 0.0f;
        }
    }
}