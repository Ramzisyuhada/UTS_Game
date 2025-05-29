using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class MoveCharacter : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    public Camera cameraTransform;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 6f;
    public float walkSprintTransition = 5f;

    private float currentSpeed;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        HandleSprint();
        Move();
    }

    private void HandleSprint()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, walkSprintTransition * Time.deltaTime);
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveDirection = Vector3.zero;

        // Rotate the player to match the camera's Y rotation
        transform.rotation = Quaternion.Euler(0f, cameraTransform.transform.eulerAngles.y, 0f);

        // Calculate movement direction based on input and player orientation
        Vector3 verticalMove = input.y * transform.forward;
        Vector3 horizontalMove = input.x * transform.right;
        moveDirection = verticalMove + horizontalMove;

        // Set Animator move speed
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        animator.SetFloat("moveSpeed", flatVelocity.magnitude);

        // Apply force for movement
        rb.AddForce(moveDirection.normalized * currentSpeed * Time.deltaTime);
    }
}
