using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;           // Reference to the player object
    public float distance = 5.0f;      // Default distance from the player
    public float height = 2.0f;        // Height of the camera relative to the player
    public float rotationSpeed = 3.0f; // Speed at which the camera rotates
    public float followSpeed = 10.0f;  // Speed at which the camera follows the player
    public float minHeightAboveGround = 0.5f; // Minimum height above the ground
    public LayerMask collisionMask;    // Layer mask for collision detection (e.g., walls, obstacles)

    private float currentRotation = 0.0f;
    private float currentVerticalRotation = 0.0f; // Vertical angle (pitch)
    private Vector3 desiredPosition;

    void Update()
    {
        // Get mouse input to rotate the camera horizontally (around the player)
        currentRotation += Input.GetAxis("Mouse X") * rotationSpeed;

        // Get mouse input to rotate the camera vertically (up and down)
        currentVerticalRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp vertical rotation to prevent the camera from flipping upside down
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, -80f, 80f);

        // Calculate the desired position of the camera in spherical coordinates (orbiting)
        Vector3 direction = new Vector3(0, height, -distance); // Direction from player (behind and above)
        Quaternion rotation = Quaternion.Euler(currentVerticalRotation, currentRotation, 0); // Combine rotations
        desiredPosition = player.position + rotation * direction; // Apply rotations to determine the desired position

        // Handle collisions using a raycast from the player to the desired camera position
        RaycastHit hit;
        if (Physics.Linecast(player.position + Vector3.up * height, desiredPosition, out hit, collisionMask))
        {
            // If there is a collision, adjust the desired position to the collision point
            desiredPosition = hit.point + hit.normal * 0.2f; // Offset slightly to avoid clipping
        }

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Ensure the camera always looks at the player
        transform.LookAt(player.position + Vector3.up * height);
    }
}