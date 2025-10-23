using UnityEngine;
using Valve.VR; // We must add this to get controller input!

/// <summary>
/// This script controls the helicopter's basic movements with a SINGLE controller
/// using its motion (position and rotation) for input.
/// It should be placed on the main "HelicopterPivot" object.
/// For this to work, the [SteamVR] rig must be in the scene, but should be separate
/// from the helicopter to allow for a fixed camera.
/// </summary>
public class HelicopterVRController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("How fast the helicopter moves forward and backward.")]
    public float moveSpeed = 15.0f;

    [Tooltip("How fast the helicopter moves up and down.")]
    public float liftSpeed = 10.0f;

    [Header("Rotation")]
    [Tooltip("How fast the helicopter rotates left and right (yaw).")]
    public float rotationSpeed = 60.0f;

    [Header("SteamVR Input Source")]
    [Tooltip("The hand you want to use for controlling the helicopter.")]
    public SteamVR_Input_Sources controlHand = SteamVR_Input_Sources.RightHand;

    [Header("SteamVR Actions")]
    [Tooltip("Action for getting the controller's position and rotation.")]
    public SteamVR_Action_Pose poseAction = SteamVR_Actions.default_Pose;

    [Tooltip("Action for moving DOWN (e.g., 'A' Button).")]
    public SteamVR_Action_Boolean liftDownAction = SteamVR_Actions.default_InteractUI;

    [Tooltip("Action for moving UP (e.g., 'B' Button).")]
    public SteamVR_Action_Boolean liftUpAction = SteamVR_Actions.default_Teleport;

    [Tooltip("Action to re-center the controller's neutral position.")]
    //public SteamVR_Action_Boolean recenterAction = SteamVR_Actions.default_JoystickClick;

    [Header("Motion Control Settings")]
    [Tooltip("How far (in meters) you move the controller for 100% thrust.")]
    public float maxMoveDistance = 0.5f; // 50cm

    [Tooltip("Movement deadzone (in meters).")]
    public float moveDeadZone = 0.05f; // 5cm

    [Tooltip("How far (in degrees) you twist your wrist for 100% rotation.")]
    public float maxTwistAngle = 45.0f; // 45 degrees

    [Tooltip("Rotation deadzone (in degrees).")]
    public float rotateDeadZone = 5.0f; // 5 degrees

    // Private variables to store the "zero" point
    private Vector3 neutralPosition;
    private Quaternion neutralRotation;

    void Start()
    {
        // Set the initial neutral position when the game starts
        RecenterController();
    }

    /// <summary>
    /// Sets the controller's current position and rotation as the new "neutral" reference.
    /// </summary>
    public void RecenterController()
    {
        // We get the position/rotation relative to the play area origin
        neutralPosition = poseAction.GetLocalPosition(controlHand);
        neutralRotation = poseAction.GetLocalRotation(controlHand);
    }

    // Update is called once per frame
    void Update()
    {
        // --- 1. CHECK FOR RE-CENTER ---
        // Allow the user to re-center by clicking the joystick
        if (recenterAction.GetStateDown(controlHand))
        {
            RecenterController();
        }

        // --- 2. GET CURRENT POSE INPUT ---
        Vector3 currentPosition = poseAction.GetLocalPosition(controlHand);
        Quaternion currentRotation = poseAction.GetLocalRotation(controlHand);

        // --- 3. GET BUTTON INPUT ---
        // Note: We swapped the actions as requested!
        bool isLiftingUp = liftUpAction.GetState(controlHand);   // 'B' Button (default_Teleport)
        bool isLiftingDown = liftDownAction.GetState(controlHand); // 'A' Button (default_InteractUI)

        // --- 4. APPLY MOVEMENT (Forward/Backward) ---
        // Calculate the difference from the neutral Z position
        float moveInput = currentPosition.z - neutralPosition.z;
        float moveAmount = 0;

        // Apply deadzone and normalize the input
        if (Mathf.Abs(moveInput) > moveDeadZone)
        {
            // Calculate how far into the "active" zone we are (from -1 to 1)
            float activeMove = moveInput - (Mathf.Sign(moveInput) * moveDeadZone);
            moveAmount = Mathf.Clamp(activeMove / (maxMoveDistance - moveDeadZone), -1.0f, 1.0f);
        }
        transform.Translate(Vector3.forward * moveAmount * moveSpeed * Time.deltaTime);


        // --- 5. APPLY ROTATION (Yaw) ---
        // Calculate the rotation difference
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(neutralRotation);
        
        // Get the twist angle around the Y (up) axis
        float twistInput = deltaRotation.eulerAngles.y;
        if (twistInput > 180) { twistInput -= 360; } // Convert from 0-360 to -180-180 range
        
        float rotateAmount = 0;

        // Apply deadzone and normalize
        if (Mathf.Abs(twistInput) > rotateDeadZone)
        {
            float activeTwist = twistInput - (Mathf.Sign(twistInput) * rotateDeadZone);
            rotateAmount = Mathf.Clamp(activeTwist / (maxTwistAngle - rotateDeadZone), -1.0f, 1.0f);
        }
        transform.Rotate(Vector3.up, rotateAmount * rotationSpeed * Time.deltaTime);


        // --- 6. APPLY ALTITUDE (Up/Down) ---
        // Use the 'B' and 'A' buttons to go up/down
        if (isLiftingUp)
        {
            transform.Translate(Vector3.up * liftSpeed * Time.deltaTime, Space.World);
        }
        else if (isLiftingDown)
        {
            transform.Translate(Vector3.down * liftSpeed * Time.deltaTime, Space.World);
        }
    }
}

