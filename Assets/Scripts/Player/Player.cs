using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

public class Player : MonoBehaviour
{
    private float speed = 5.0f;
    private Vector2 movement = Vector2.zero;
    private float moveX = 0.0f;
    private float moveY = 0.0f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isPaused = false;
    [SerializeField] private BulletBehavior BulletPrefab;
    [SerializeField] private Transform LaunchOffset;
    [SerializeField] private AudioSource bulletAudio;
    [SerializeField] private AudioClip bulletClip;
    public IInteractable Interactable { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            ProcessInput();
        }
    }
    private void ProcessInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            bulletAudio.PlayOneShot(bulletClip);
            Instantiate(BulletPrefab, LaunchOffset.position, transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        Move();
        Animate();
    }

    private void Move()
    {
        movement = new Vector2(moveX, moveY).normalized;
        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
    }
    private void Animate()
    {
        anim.SetFloat("direction", movement.y);
    }
}
