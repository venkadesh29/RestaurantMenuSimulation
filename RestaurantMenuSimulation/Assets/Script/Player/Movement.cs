using System.Collections;
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
    private PlayerOrder playerOrder; // Reference to the PlayerOrder component

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerOrder = GetComponent<PlayerOrder>(); // Get PlayerOrder component

        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        // Check for chair interaction
        raycast();

        // Prevent movement if locked
        if (!canMove) return;

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
        isRotatingToSit = true; // Lock rotation state

        // Sit down directly
        isSitting = true; // Set sitting state
        Debug.Log(name + " has sat down.");

        // Call SitDown method in PlayerOrder
        playerOrder.SitDown();

        yield return null; // Just a yield to return to the coroutine caller
    }

    public void StandUp()
    {
        if (isSitting)
        {
            StartCoroutine(StandUpCoroutine()); // Start the coroutine for standing up
        }
    }

    private IEnumerator StandUpCoroutine()
    {
        yield return new WaitForSeconds(1f); // Wait for standing animation to finish
        isSitting = false; // Reset sitting state
        playerOrder.StandUp(); // Call StandUp method in PlayerOrder
        Debug.Log("Player is standing up."); // Log the action
    }

    // Add your other methods and logic as needed
}
