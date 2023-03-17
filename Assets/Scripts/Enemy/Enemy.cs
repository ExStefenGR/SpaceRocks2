using System.Collections;
using UnityEngine;

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
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject target;
    private EnemyState randomState;
    private Transform targetTransform;

    // Enum for states
    private enum EnemyState
    {
        MoveInLine,
        Follow,
        FireInLine,
        FireFollow
    };

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        randomState = (EnemyState)Random.Range(0, 4);
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        target = GameObject.Find("Player");
        targetTransform = target.transform;

        if (randomState == EnemyState.FireInLine || randomState == EnemyState.FireFollow)
        {
            InvokeRepeating("Shoot", 0, fireRate);
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
                movement.x = -1.0f;
                movement.y = 0.0f;
                break;
            case EnemyState.Follow:
                movement = Vector2.MoveTowards(transform.position, targetTransform.position, speed * Time.fixedDeltaTime) - (Vector2)transform.position;
                break;
            case EnemyState.FireInLine:
                movement.x = -1.0f;
                movement.y = 0.0f;
                break;
            default:
                movement.x = -1.0f;
                movement.y = 0.0f;
                break;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
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
        Debug.Log("Enemy OnCollisionEnter2D with " + collision.gameObject.tag);
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag("bullet"))
        {
            hp--;
            if (hp <= 0)
            {
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
        Instantiate(bulletPrefab, launchOffset.position, launchOffset.rotation);
        bulletAudio.PlayOneShot(bulletClip);
    }
}