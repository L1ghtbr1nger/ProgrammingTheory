using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 150f;
    [SerializeField] private float pitchMin = -80f;
    [SerializeField] private float pitchMax = 80f;

    [Header("Camera Follow (First Person Offset)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 3.5f, 1f);
    [SerializeField] private bool smoothCamera = true;
    [SerializeField] private float rotationSmoothTime = 0.08f;
    [SerializeField] private float positionSmoothTime = 0.08f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float pitch;
    private bool sprinting;

    // smoothing state
    private Vector3 cameraPositionVelocity;
    private float currentYaw;
    private float currentYawVelocity;
    private float currentPitch;
    private float currentPitchVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        if (cameraTransform != null)
        {
            var e = cameraTransform.localEulerAngles;
            // normalize to signed angle to avoid jumps
            float ex = e.x;
            if (ex > 180f) ex -= 360f;
            pitch = ex;

            // initialize smoothing state to current camera orientation
            currentPitch = pitch;
            currentYaw = transform.eulerAngles.y;
        }
    }

    public void SetMoveInput(Vector2 value) => moveInput = value;
    public void SetLookInput(Vector2 value) => lookInput = value;
    public void SetSprint(bool value) => sprinting = value;

    public void Jump()
    {
        if (controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Update()
    {
        // Look & rotate character with camera yaw
        if (lookInput.sqrMagnitude > 0.0001f)
        {
            // Yaw rotates the character
            transform.Rotate(0f, lookInput.x * lookSensitivity * Time.deltaTime, 0f);

            // Pitch only affects the camera target
            if (cameraTransform != null)
            {
                pitch -= lookInput.y * lookSensitivity * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
            }
        }

        // Move relative to look (camera) direction on ground plane
        Vector3 fwd = (cameraTransform != null ? cameraTransform.forward : transform.forward);
        Vector3 right = (cameraTransform != null ? cameraTransform.right : transform.right);
        fwd.y = 0f; right.y = 0f;
        fwd.Normalize(); right.Normalize();

        Vector3 planar = (fwd * moveInput.y + right * moveInput.x).normalized;
        float speed = sprinting ? sprintSpeed : moveSpeed;
        Vector3 velocity = planar * speed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // keep grounded
        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        // compute targets
        Vector3 targetPos = transform.TransformPoint(cameraOffset);
        float targetYaw = transform.eulerAngles.y;
        float targetPitch = pitch;

        if (smoothCamera)
        {
            // Smooth yaw and pitch
            currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref currentYawVelocity, rotationSmoothTime);
            currentPitch = Mathf.SmoothDampAngle(currentPitch, targetPitch, ref currentPitchVelocity, rotationSmoothTime);

            cameraTransform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPos, ref cameraPositionVelocity, positionSmoothTime);
        }
        else
        {
            cameraTransform.position = targetPos;
            cameraTransform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0f);

            // keep smoothing state in sync
            currentYaw = targetYaw;
            currentPitch = targetPitch;
            currentYawVelocity = 0f;
            currentPitchVelocity = 0f;
            cameraPositionVelocity = Vector3.zero;
        }
    }
}