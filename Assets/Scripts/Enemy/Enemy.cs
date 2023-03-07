using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 movement = Vector2.zero;
    private Vector2 move = Vector2.zero;
    private Rigidbody2D rb;
    private Animator anim;
    public bool isPaused = false;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private EnemyBullet BulletPrefab;
    [SerializeField] private Transform LaunchOffset;
    [SerializeField] private AudioSource bulletAudio;
    [SerializeField] private AudioClip bulletClip;
    //Enemy Parameters
    [SerializeField] private int hp = 1;
    [SerializeField] private int waitForFire;
    [SerializeField] private bool fire = false;
    [SerializeField] private GameObject target;
    private EnemyState randomState;

    //enum for states
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
        randomState = (EnemyState)Random.Range(0, 3); //Gets a random state once
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isPaused)
        {
            ProcessInput();
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    //Behaviour here
    private void ProcessInput()
    {
        if (randomState == EnemyState.MoveInLine)
        {
            move.x = -1.0f;
        }
        else if (randomState == EnemyState.Follow)
        {
            move.x = target.transform.position.x;
            move.y = target.transform.position.y;
        }
        else if (randomState == EnemyState.FireInLine)
        {
            move.x = -1.0f;
            if (Random.Range(0, 100) >= 70 & !fire)
            {
                _ = StartCoroutine(Shoot());
            }
        }
        else
        {
            move.x = -1.0f;
        }

    }

    private void FixedUpdate()
    {
        Move();
        Animate();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullet"))
        {
            hp--;
        }
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("EnemyBarrier"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Enemybullet"))
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemybarrier"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemybullet"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Shoot()
    {
        fire = true;
        _ = Instantiate(BulletPrefab, LaunchOffset.position, LaunchOffset.rotation);
        bulletAudio.PlayOneShot(bulletClip);
        yield return new WaitForSeconds(waitForFire);
        yield return fire = false;
    }
}
