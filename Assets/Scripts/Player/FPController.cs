using Mirror;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPController : NetworkBehaviour
{
    [Header("Movement Parameters")]
    public float MaxSpeed = 3.5f;
    public float Acceleration = 15f;

    [Space(15)]
    [Tooltip("The height the player can jump in units.")]
    [SerializeField] float JumpHeight = 2f;

    [Header("Looking Parameters")]
    public Vector2 LookSensitivity = new Vector2(0.1f, 0.1f);

    public float pitchLimit = 85f;
    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    {
        get => currentPitch;
        set
        {
            currentPitch = Mathf.Clamp(value, -pitchLimit, pitchLimit);
        }
    }

    [Header("Physics Parameters")]
    [SerializeField] float GravityScale = 3f;
    public float VerticalVelocity { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded => characterController.isGrounded;

    [Header("Input")]
    public Vector2 MoveInput;
    public Vector2 LookInput;

    [Header("Components")]
    [SerializeField] CinemachineCamera fpCamera;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator PlayerAnimator;

    #region Unity Methods

    void Update()
    {
        if (!isLocalPlayer) return;
        MoveUpdate();
        LookUpdate();
    }

    public override void OnStartClient()
    {
        // Make sure remote players don't run local input/camera
        if (!isLocalPlayer)
        {
            if (fpCamera != null)
            {
                fpCamera.enabled = false;
                var listener = fpCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = false;
            }

            // Prevent remote objects from capturing local input
            if (fpCamera != null) fpCamera.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        // Enable camera + audio only for the local player
        // if (Camera.main != null)
        // {
        //     Camera.main.enabled = true;
        //     var listener = Camera.main.GetComponent<AudioListener>();
        //     if (listener != null) listener.enabled = true;
        // }

        // // Enable PlayerInput for local player
        // if (Camera.main != null) Camera.main.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    #endregion

    #region Controller Methods

    public void TryJump()
    {
        if (IsGrounded)
        {
            VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y * GravityScale);
        }
    }

    void MoveUpdate()
    {
        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        motion.y = 0f;
        motion.Normalize();

        if (motion.sqrMagnitude >= 0.1f)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * MaxSpeed, Acceleration * Time.deltaTime);
            PlayerAnimator.SetBool("IsRunning", true);
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
            PlayerAnimator.SetBool("IsRunning", false);
        }

        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = -3f;

        }
        else
        {
            VerticalVelocity += Physics.gravity.y * GravityScale * Time.deltaTime;
        }

        Vector3 fullVelocity = new(CurrentVelocity.x, VerticalVelocity, CurrentVelocity.z);

        characterController.Move(fullVelocity * Time.deltaTime);

        CurrentSpeed = CurrentVelocity.magnitude;
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x, LookInput.y * LookSensitivity.y);

        // Looking up and down
        CurrentPitch -= input.y;
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        // Looking left and right
        transform.Rotate(Vector3.up * input.x);
    }

    #endregion
}
