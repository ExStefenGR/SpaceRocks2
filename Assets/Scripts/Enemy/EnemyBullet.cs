using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemybarrier"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("barrier"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
