using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 720f;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float currentSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        Move();
        Animate();
    }

    void HandleInput()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate move direction
        moveDirection = new Vector3(h, 0, v).normalized;

        // Determine speed
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move and rotate character
        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(move * currentSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
            characterController.Move(move);
        }
    }

    void Animate()
    {
        // Set animation parameters
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetFloat("Direction", Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up));
    }
}
