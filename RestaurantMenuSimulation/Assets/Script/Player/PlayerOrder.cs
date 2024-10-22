using UnityEngine;

public class PlayerOrder : MonoBehaviour
{
    public WaiterNPC waiterNPC; // Assign the waiter NPC in the Inspector
    public GameObject orderCanvas; // The canvas to assign in the Inspector
    public Camera playerCamera; // Reference to the player camera
    public Transform canvasTransform; // Reference to the canvas transform

    private bool isSitting = false; // Track if the player is sitting

    private string[] foodOptions = { "Pizza", "Burger", "Pasta", "Salad" }; // Food options array

    void Start()
    {
        orderCanvas.SetActive(false); // Start with the canvas inactive
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor at the start
    }

    void Update()
    {
        // Open canvas when player is sitting and presses 'C'
        if (isSitting && Input.GetKeyDown(KeyCode.C))
        {
            ShowOrderCanvas();
        }

        // Close canvas when 'ESC' is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCanvas();
        }
    }

    // Called when the player sits down
    public void SitDown()
    {
        isSitting = true;
        ShowOrderCanvas(); // Open the canvas once the player sits
    }

    // Called when the player stands up
    public void StandUp()
    {
        isSitting = false;
        CloseCanvas(); // Close the canvas when standing up
    }

    // Method to show the order canvas
    private void ShowOrderCanvas()
    {
        orderCanvas.SetActive(true); // Open the canvas
        MoveCameraToCanvas(); // Move camera to canvas
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }

    // Method to close the canvas
    public void CloseCanvas()
    {
        orderCanvas.SetActive(false); // Close the canvas
        MoveCameraToPlayer(); // Return camera to player
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor again
    }

    private void MoveCameraToCanvas()
    {
        if (playerCamera != null && canvasTransform != null)
        {
            playerCamera.transform.position = canvasTransform.position + new Vector3(0, 2, -5); // Adjust the offset as needed
            playerCamera.transform.LookAt(canvasTransform); // Make the camera look at the canvas
        }
    }

    private void MoveCameraToPlayer()
    {
        if (playerCamera != null)
        {
            playerCamera.transform.position = transform.position + new Vector3(0, 2, -5); // Adjust the offset as needed
            playerCamera.transform.LookAt(transform); // Make the camera look at the player
        }
    }

    // Food ordering logic
    public void OrderFood(int foodIndex)
    {
        if (foodIndex < 0 || foodIndex >= foodOptions.Length)
        {
            Debug.LogError("Invalid food index!");
            return;
        }

        string food = foodOptions[foodIndex];
        Debug.Log("Ordered: " + food); // Log the order

        // Notify the waiter to serve food
        waiterNPC.SetTarget(transform); // Set the player as the target

        // Close the canvas once the food is ordered
        CloseCanvas();
    }

    // Button click methods for each food item
    public void OrderPizza() 
    {
        OrderFood(0); // Pizza is at index 0
    }

    public void OrderBurger() 
    {
        OrderFood(1); // Burger is at index 1
    }

    public void OrderPasta() 
    {
        OrderFood(2); // Pasta is at index 2
    }

    public void OrderSalad() 
    {
        OrderFood(3); // Salad is at index 3
    }

    // Example method to demonstrate player interaction (e.g., movement lock/unlock)
    public void EatFood()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("eat");

        // After eating, unlock movement and allow standing up
        StandUp();
    }
}
