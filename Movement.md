<h1 align="center">Movement Class Documentation</h1>
<h3 align="center">Control Character Movement in Unity</h3>

<div style="text-align: justify;">
<p>The <strong>Movement</strong> class controls character movement in a 3D environment using Unity's CharacterController component. It includes functionality for movement, rotation, sitting, standing up, and interacting with objects like chairs.</p>
</div>

<h2>Fields</h2>
<details>
<summary>Click to expand</summary>
<ul>
    <li><strong>[Header("Movement")]</strong>
        <ul>
            <li><code>public float moveSpeed = 5f;</code>: Speed of the character's movement.</li>
            <li><code>public float rotationSpeed = 10f;</code>: Speed of the character's rotation.</li>
            <li><code>private Vector3 moveDirection = Vector3.zero;</code>: Current direction of movement.</li>
            <li><code>private CharacterController characterController;</code>: Reference to the CharacterController component.</li>
        </ul>
    </li>
    <li><strong>[Header("Mouse")]</strong>
        <ul>
            <li><code>public float mouseSpeed = 100f;</code>: Speed of mouse movement affecting rotation.</li>
            <li><code>public float y_axis = 0f;</code>: The current Y-axis rotation of the character.</li>
        </ul>
    </li>
    <li><strong>[Header("Sitting")]</strong>
        <ul>
            <li><code>private bool isSitting = false;</code>: Indicates whether the character is currently sitting.</li>
            <li><code>private bool isRotatingToSit = false;</code>: Indicates if the character is in the process of rotating to sit.</li>
            <li><code>private bool canMove = true;</code>: Indicates whether the character is allowed to move.</li>
        </ul>
    </li>
    <li><strong>[Header("Raycast")]</strong>
        <ul>
            <li><code>public float rayLength = 1f;</code>: Length of the ray for detecting chairs.</li>
        </ul>
    </li>
    <li><strong>[Header("Animation")]</strong>
        <ul>
            <li><code>private Animator animator;</code>: Reference to the Animator component for handling animations.</li>
        </ul>
    </li>
</ul>
</details>

<h2>Methods</h2>
<details>
<summary>Click to expand</summary>
<ul>
    <li><strong>void Start()</strong>
        <p><em>Description:</em> Initializes the CharacterController, Animator, locks the cursor, and sets the initial idle animation.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Assigns the CharacterController and Animator components.</li>
                <li>Locks the cursor to the center of the screen for better control.</li>
                <li>Sets the initial animation state to idle.</li>
            </ul>
        </p>
    </li>
    <li><strong>void Update()</strong>
        <p><em>Description:</em> Called once per frame, handles input, movement, and raycasting for chair detection.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Calls raycast() to check for nearby chairs.</li>
                <li>Calls movement() to handle character movement.</li>
                <li>If the character is sitting and a movement key is pressed, it calls StandUp().</li>
            </ul>
        </p>
    </li>
    <li><strong>void movement()</strong>
        <p><em>Description:</em> Manages character movement based on the current state (sitting or standing).</p>
        <p><em>Flow:</em>
            <ul>
                <li>If movement is not allowed (!canMove) or the character is currently rotating to sit, exit the method.</li>
                <li>If the character is sitting, call HandleSittingMovement().</li>
                <li>If not sitting, call HandleRegularMovement().</li>
            </ul>
        </p>
    </li>
    <li><strong>private void HandleSittingMovement()</strong>
        <p><em>Description:</em> Handles the state when the character is sitting.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Sets the moveDirection to zero, preventing movement while sitting.</li>
                <li>Optionally calls detectAnimation() to trigger any sitting animations or logic.</li>
            </ul>
        </p>
    </li>
    <li><strong>private void HandleRegularMovement()</strong>
        <p><em>Description:</em> Handles character movement when the character is not sitting.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Retrieves horizontal and vertical input for movement.</li>
                <li>If the character is not sitting, retrieves mouse input to update the character's rotation.</li>
                <li>Normalizes the movement direction based on input.</li>
                <li>Applies gravity if the character is not grounded.</li>
                <li>Moves the character using the CharacterController.</li>
                <li>Updates the animation state based on the current movement speed.</li>
            </ul>
        </p>
    </li>
    <li><strong>void raycast()</strong>
        <p><em>Description:</em> Casts a ray forward to check for chairs within range.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Uses Physics.Raycast to detect if a chair is in front of the character.</li>
                <li>If a chair is detected, calls detectedChair() to handle the interaction.</li>
                <li>Draws a debug ray in the scene for visualization.</li>
            </ul>
        </p>
    </li>
    <li><strong>void detectedChair()</strong>
        <p><em>Description:</em> Handles the detection of chairs and initiates the sitting process if the player presses 'F'.</p>
        <p><em>Flow:</em>
            <ul>
                <li>If 'F' is pressed, and the character is not currently sitting or rotating to sit, it logs the event and starts the sitting rotation coroutine.</li>
            </ul>
        </p>
    </li>
    <li><strong>private IEnumerator RotateToSit()</strong>
        <p><em>Description:</em> Rotates the character towards the chair and transitions to the sitting state.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Sets isRotatingToSit to true to prevent other actions.</li>
                <li>Calculates the target rotation to face the chair.</li>
                <li>Gradually rotates the character towards the target rotation.</li>
                <li>Updates the y_axis once the rotation is complete.</li>
                <li>Sets isSitting to true and calls detectAnimation().</li>
            </ul>
        </p>
    </li>
    <li><strong>void detectAnimation()</strong>
        <p><em>Description:</em> Placeholder for animation logic related to sitting.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Currently logs that the sitting animation is detected. This is where you can trigger the sitting animation.</li>
            </ul>
        </p>
    </li>
    <li><strong>private void StandUp()</strong>
        <p><em>Description:</em> Transitions the character from sitting to standing and waits before allowing movement.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Sets isSitting to false and canMove to false to prevent immediate movement.</li>
                <li>Starts a coroutine to wait for a duration before allowing movement again.</li>
            </ul>
        </p>
    </li>
    <li><strong>private IEnumerator WaitForMovement(float delay)</strong>
        <p><em>Description:</em> Waits for a specified duration before re-enabling character movement.</p>
        <p><em>Flow:</em>
            <ul>
                <li>Waits for the specified delay using yield return new WaitForSeconds(delay).</li>
                <li>After the wait, sets canMove to true, allowing movement again.</li>
            </ul>
        </p>
    </li>
</ul>
</details>

<h2>Mechanics</h2>
<details>
<summary>Click to expand</summary>
<ul>
    <li><strong>Sitting Animation:</strong>
        <p>The sitting animation is triggered when the player interacts with a chair by pressing 'F'. The character rotates towards the chair using the <code>RotateToSit()</code> coroutine. The character's rotation is handled gradually to provide a smooth transition:</p>
        <pre><code>while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }</code></pre>
        <p>This rotation logic ensures the character is correctly oriented towards the chair before triggering any animations.</p>
    </li>
    <li><strong>Movement Controls:</strong>
        <p>The character movement is controlled via Unity's input system. Movement is calculated based on the horizontal and vertical input axes. The character is prevented from moving while sitting, ensuring a realistic experience:</p>
        <pre><code>if (!isSitting) {
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    moveDirection = new Vector3(horizontal, 0, vertical).normalized;
}</code></pre>
        <p>This approach allows for standard WASD movement in the game.</p>
    </li>
    <li><strong>Raycasting for Chairs:</strong>
        <p>The raycasting mechanism is implemented to detect chairs in front of the character:</p>
        <pre><code>if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength)) {
    if (hit.collider.CompareTag("Chair")) {
        detectedChair();
    }
}</code></pre>
        <p>This checks if the object in front of the player has the tag "Chair," facilitating easy interactions.</p>
    </li>
</ul>
</details>

<h2>Conclusion</h2>
<p>This Movement class provides a robust system for controlling character movement and interactions in a Unity 3D environment. It supports both walking and sitting mechanics, allowing for dynamic gameplay experiences.</p>
