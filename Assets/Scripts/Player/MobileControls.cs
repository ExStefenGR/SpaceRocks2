using UnityEngine;

public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance { get; private set; }

    public float movementThreshold = 0.1f;

    private bool isFiring;
    private bool isMoving;
    private Vector2 moveInput;

    public bool IsFiring => isFiring;
    public bool IsMoving => isMoving;
    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Reset input values
        isFiring = false;
        isMoving = false;

        // Process touch input
        foreach (Touch touch in Input.touches)
        {
            if (touch.position.x < Screen.width / 2f)
            {
                // Left side of the screen is for movement
                moveInput = (touch.position - new Vector2(Screen.width / 4f, Screen.height / 2f)).normalized;
                isMoving = true;
            }
            else
            {
                // Right side of the screen is for firing
                isFiring = true;
            }
        }
    }
}
