using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Movement settings
    [Header("Movement")]
    public float moveSpeed = 5f; // Speed of the character's movement
    public float rotationSpeed = 10f; // Speed of rotation
    private Vector3 moveDirection = Vector3.zero; // Current direction of movement
    private CharacterController characterController; // Reference to the CharacterController

    // Mouse control settings
    [Header("Mouse")]
    public float mouseSpeed = 100f; // Mouse movement speed
    public float y_axis = 0f; // Y-axis rotation

    // Sitting states
    [Header("Sitting")]
    private bool isSitting = false; // Check if the character is sitting
    private bool isRotatingToSit = false; // Check if the character is currently rotating to sit
    private bool canMove = true; // Flag to control movement

    // Raycast settings
    [Header("Raycast")]
    public float rayLength = 1f; // Length of ray for detecting objects

    // Animation component
    [Header("Animation")]
    private Animator animator; // Reference to Animator component

    void Start()
    {
        // Initialize the character controller and animator
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked; 
        animator.SetBool("Idle", true); // Set initial animation state to idle
    }

    void Update()
    {
        // Check for chair interaction
        raycast();

        // Handle movement logic
        movement();

        // If sitting and movement keys are pressed, stand up
        if (isSitting && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
        {
            StandUp(); // Transition to standing state
        }
    }

    void movement()
    {
        // Prevent movement if not allowed or currently rotating to sit
        if (!canMove || isRotatingToSit) return;

        // If sitting, handle sitting movement
        if (isSitting)
        {
            HandleSittingMovement();
            return; // Skip further processing while sitting
        }

        // If standing, handle regular movement
        HandleRegularMovement();
    }

    private void HandleSittingMovement()
    {
        // Prevent any movement while sitting
        moveDirection = Vector3.zero;
        detectAnimation(); // Optionally trigger sitting animation
    }

    private void HandleRegularMovement()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Only allow mouse movement when not sitting
        if (!isSitting)
        {
            float mouseMovement = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime; // Get mouse movement
            y_axis += mouseMovement; // Update Y-axis rotation
            transform.rotation = Quaternion.Euler(0f, y_axis, 0f); // Apply rotation to character
        }

        // Calculate movement direction based on input
        moveDirection = new Vector3(horizontal, 0, verticalInput).normalized; // Normalize input
        Vector3 movementNormal = transform.TransformDirection(moveDirection); // Transform direction into world space

        // Apply gravity if not grounded
        if (!characterController.isGrounded)
        {
            movementNormal.y -= 9.81f * Time.deltaTime; // Apply gravity
        }

        // Move the character based on calculated direction and speed
        characterController.Move(movementNormal * moveSpeed * Time.deltaTime);

        // Calculate speed and update animation states
        float speed = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude; // Calculate current speed

        // Set animation states based on speed
        if (speed > 0.01f)
        {
            animator.SetBool("Walk", true); // Trigger walking animation
            animator.SetBool("Idle", false); // Ensure idle animation is not playing
        }
        else
        {
            animator.SetBool("Walk", false); // Stop walking animation
            animator.SetBool("Idle", true); // Trigger idle animation
        }
    }

    void raycast()
    {
        RaycastHit hit; // Variable to store raycast hit information
        Vector3 rayDirection = transform.forward; // Direction of the ray

        // Cast a ray to detect objects in front of the character
        if (Physics.Raycast(transform.position, rayDirection, out hit, rayLength))
        {
            // Check if the detected object is a chair
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Chair"))
            {
                detectedChair(); // Call chair detection method
            }
        }

        // Draw the ray in the scene for debugging purposes
        Debug.DrawRay(transform.position, rayDirection * rayLength, Color.red);
    }

    void detectedChair()
    {
        // Check for interaction input when near a chair
        if (Input.GetKeyDown(KeyCode.F) && !isSitting && !isRotatingToSit)
        {
            Debug.Log("Preparing to sit..."); // Log the action
            StartCoroutine(RotateToSit()); // Start the coroutine to rotate and sit
        }
    }

    private IEnumerator RotateToSit()
    {
        isRotatingToSit = true; // Prevent movement while rotating
        Quaternion targetRotation = Quaternion.Euler(0, y_axis + 180f, 0f); // Calculate target rotation

        // Rotate towards the target rotation gradually
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Rotate character
            yield return null; // Wait for the next frame
        }

        // Update y_axis after rotation is complete
        y_axis = transform.rotation.eulerAngles.y; 
        isRotatingToSit = false; // Allow sitting after rotation completes

        isSitting = true; // Set the sitting state to true
        detectAnimation(); // Call animation detection method
        Debug.Log("Now sitting"); // Log the sitting action
    }

    void detectAnimation()
    {
        Debug.Log("Sitting animation"); // Log that the sitting animation is detected
    }

    private void StandUp()
    {
        isSitting = false; // Allow standing up
        canMove = false; // Disable movement temporarily
        Debug.Log("Standing up"); // Log standing up action

        // Uncomment to trigger standing animations
        // animator.SetBool("Sit", false); 
        // animator.SetBool("Idle", true); 

        StartCoroutine(WaitForMovement(2f)); // Start coroutine to wait before allowing movement
    }

    private IEnumerator WaitForMovement(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        canMove = true; // Re-enable movement after the wait
        Debug.Log("Movement enabled after standing up"); // Log that movement is now enabled
    }
}
