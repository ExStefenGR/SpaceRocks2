using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private readonly float moveX = 1;
    [SerializeField] private float speed;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {

        rb.velocity = new(moveX * speed, 0.0f);
    }
}
