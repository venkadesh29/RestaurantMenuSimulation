using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotSpeed = 500f;
    public float rotationSpeed = 10f; // Increased rotation speed for faster turning
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    [Header("Mouse")]
    public float mouseSpeed = 100f;
    public float y_axis = 0f;

    [Header("Sitting")]
    private bool isSitting = false;

    [Header("Raycast")]
    public float rayLength = 1f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        raycast();        
        movement();
    }

    void movement()
    {
        if (isSitting)
        {
            float vertical = Input.GetAxis("Vertical");

            // Calculate the movement direction based on current rotation
            moveDirection = transform.forward * vertical; // Forward direction based on character's rotation

            if (!characterController.isGrounded)
            {
                moveDirection.y -= 9.81f * Time.deltaTime; // Apply gravity
            }

            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            return; // Prevent further processing for sitting
        }

        // Normal movement processing when not sitting
        float horizontal = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float mouseMovement = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        y_axis += mouseMovement; // Update Y-axis rotation
        transform.rotation = Quaternion.Euler(0f, y_axis, 0f); // Apply rotation

        // Calculate the movement direction based on the character's new rotation
        moveDirection = new Vector3(horizontal, 0, verticalInput).normalized;
        Vector3 movementNormal = transform.TransformDirection(moveDirection); 

        // Combine horizontal movement and vertical input
        moveDirection = movementNormal + Vector3.down * (characterController.isGrounded ? 0 : 9.81f); // Apply gravity if not grounded

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void raycast()
    {
        RaycastHit hit;
        Vector3 rayDirection = transform.forward;
        

        if (Physics.Raycast(transform.position, rayDirection, out hit, rayLength))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Chair"))
            {
                detectedChair();
            }
        }

        Debug.DrawRay(transform.position, rayDirection * rayLength, Color.red);
    }

    void detectedChair()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isSitting = true;
            Debug.Log("Sitting down");
            StartCoroutine(RotateToSit());
        }
        else if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Movement Resumed");
        }
    }

    private IEnumerator RotateToSit()
    {
        Quaternion targetRotation = Quaternion.Euler(0, y_axis + 180f, 0f); // Correctly target the new rotation
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Update y_axis to the new forward direction after the rotation
        y_axis += 180f; 
        detectAnimation();
    }

    void detectAnimation()
    {
        Debug.Log("Sitting animation");
        isSitting = false; // Reset sitting status to allow movement
    }
}
