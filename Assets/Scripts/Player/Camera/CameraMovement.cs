using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveX = 1;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {

        rb.velocity = new Vector2(moveX * speed, 0.0f);
    }
}
