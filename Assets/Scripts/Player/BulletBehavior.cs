using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
