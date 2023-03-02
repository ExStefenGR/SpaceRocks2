using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + new Vector3(1.0f * velocity * Time.deltaTime, 0.0f,0.0f);
    }
}
