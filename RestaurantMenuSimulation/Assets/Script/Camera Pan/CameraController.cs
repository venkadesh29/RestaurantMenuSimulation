using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Transform canvasPosition; // The position where the camera should focus when showing the canvas
    public Transform defaultPosition; // The default camera position
    public float panSpeed = 5f; // Speed of the camera panning

    private bool isPanningToCanvas = false; // Flag to check if the camera is panning to the canvas
    private bool isCanvasActive = false; // Flag to check if the canvas is currently active
    private bool isPanningBack = false; // Flag to check if the camera is panning back to the player

    public GameObject canvas; // Reference to the canvas GameObject

    void Start()
    {
        canvas.SetActive(false); // Deactivate canvas at the start
    }

    void Update()
    {
        // Check if the player sits (trigger this from your player script)
        if (Input.GetKeyDown(KeyCode.F)) // Replace this with your sit detection
        {
            SitAndActivateCanvas();
        }

        // Reopen the canvas if additional food is needed (only if the canvas is already active)
        if (Input.GetKeyDown(KeyCode.C) && isCanvasActive)
        {
            OpenCanvas();
        }

        // Close the canvas and return camera to player
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCanvasAndReturn();
        }

        // Pan the camera to the canvas
        if (isPanningToCanvas)
        {
            PanToPosition(canvasPosition.position);
        }

        // Pan the camera back to the player
        if (isPanningBack)
        {
            PanToPosition(defaultPosition.position);
        }
    }

    void SitAndActivateCanvas()
    {
        // Activate the canvas once the player sits
        canvas.SetActive(true);
        isCanvasActive = true;
        isPanningToCanvas = true;
    }

    void PanToPosition(Vector3 targetPosition)
    {
        // Smoothly pan the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, panSpeed * Time.deltaTime);

        // Check if the camera is near the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Stop panning once the target position is reached
            if (isPanningToCanvas)
            {
                isPanningToCanvas = false;
            }
            else if (isPanningBack)
            {
                isPanningBack = false;
            }
        }
    }

    public void ServeFood()
    {
        // Deactivate the canvas after serving the food
        canvas.SetActive(false);
        isCanvasActive = false;
        isPanningBack = true; // Pan the camera back to the player after serving
    }

    void OpenCanvas()
    {
        // Reactivate the canvas if additional food is needed
        canvas.SetActive(true);
        isCanvasActive = true;
        isPanningToCanvas = true;
    }

    private void CloseCanvasAndReturn()
    {
        // Deactivate the canvas and return the camera to the player
        canvas.SetActive(false);
        isCanvasActive = false;
        isPanningBack = true; // Start panning back to the player
    }
}
