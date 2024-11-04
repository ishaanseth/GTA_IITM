using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player's position
    public float smoothing = 5f;  // Damping effect for smooth movement

    private Vector3 offset;  // Distance between the player and camera

    void Start()
    {
        // Calculate and store the offset between the player and the camera
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // Desired camera position based on player's position and the offset
        Vector3 targetPosition = player.position + offset;

        targetPosition.z = transform.position.z;  // Keep the camera's Z-axis constant

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}
