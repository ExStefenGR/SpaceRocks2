using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody2D rb;
    private ParticleSystem pSystem;
    private int bulletHp = 1; //Will let charged bullet pass through one ship

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("barrier") || collision.gameObject.CompareTag("Enemy"))
        {
            pSystem.Emit(20);
            gameObject.SetActive(false);
        }
    }
}