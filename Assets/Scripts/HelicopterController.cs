using UnityEngine;

/// <summary>
/// This script controls the helicopter's basic movements using the keyboard.
/// It uses Transform.Translate and Transform.Rotate for direct control, not physics.
/// </summary>
public class HelicopterController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("How fast the helicopter moves forward and backward.")]
    public float moveSpeed = 15.0f;

    [Tooltip("How fast the helicopter moves up and down.")]
    public float liftSpeed = 10.0f;

    [Header("Rotation")]
    [Tooltip("How fast the helicopter rotates left and right (yaw).")]
    public float rotationSpeed = 60.0f;

    // Update is called once per frame
    void Update()
    {
        // --- FORWARD & BACKWARD MOVEMENT (Up/Down Arrows) ---
        // We use else-if to prevent moving forward and backward at the same time.
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Move forward along the helicopter's local Z-axis
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // Move backward along the helicopter's local Z-axis
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        // --- ROTATION (Left/Right Arrows) ---
        // This controls the "yaw" of the helicopter.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Rotate left around the local Y-axis (up)
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // Rotate right around the local Y-axis (up)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // --- ALTITUDE (Up/Down) ---
        // Since Up/Down arrows are for movement, we'll use 'W' and 'S' for altitude.
        // This is a common control scheme.
        if (Input.GetKey(KeyCode.W))
        {
            // Move straight up, relative to the world
            transform.Translate(Vector3.up * liftSpeed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Move straight down, relative to the world
            transform.Translate(Vector3.down * liftSpeed * Time.deltaTime, Space.World);
        }
    }
}
