using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FirstPersonMirrorController_InputSystem : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    [Header("Look")]
    public Transform cameraPivot;     // child transform where camera sits
    public Camera playerCamera;       // camera on the player
    public float mouseSensitivity = 0.12f; // tune for your feel
    public float pitchMin = -80f;
    public float pitchMax = 80f;

    private CharacterController cc;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private Vector3 velocity;
    private float pitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnStartClient()
    {
        // Make sure remote players don't run local input/camera
        if (!isLocalPlayer)
        {
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                var listener = playerCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = false;
            }

            // Prevent remote objects from capturing local input
            if (playerInput != null) playerInput.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        // Enable camera + audio only for the local player
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            var listener = playerCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }

        // Enable PlayerInput for local player
        if (playerInput != null) playerInput.enabled = true;

        // Cache actions (names must match your Input Actions asset)
        moveAction = playerInput.actions.FindAction("Move", throwIfNotFound: true);
        lookAction = playerInput.actions.FindAction("Look", throwIfNotFound: true);
        jumpAction = playerInput.actions.FindAction("Jump", throwIfNotFound: true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        Look();
        Move();
    }

    void Look()
    {
        // Mouse Delta is usually "pixels per frame" style data (don’t multiply by deltaTime)
        Vector2 lookDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        // Yaw on body
        transform.Rotate(Vector3.up * lookDelta.x);

        // Pitch on camera pivot
        pitch -= lookDelta.y;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;

        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Input System: use WasPressedThisFrame for a button action
        if (cc.isGrounded && jumpAction.WasPressedThisFrame())
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        cc.Move((move + velocity) * Time.deltaTime);
    }
}
