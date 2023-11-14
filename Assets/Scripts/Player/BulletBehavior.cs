using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    private Rigidbody2D rb;
    private ParticleSystem pSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        rb.velocity = transform.up * speed;
        pSystem.Emit(20);
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("barrier") || collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
}