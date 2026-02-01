using Mirror;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using static TrapEnum;

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

    [Space(15)]
    [SerializeField] GameObject playerModel;

    [Header("First Person Visuals")]
    [SerializeField] bool hideBodyForOwner = true;

    [Tooltip("If true, your body mesh is invisible but still casts shadows.")]
    [SerializeField] bool keepShadowsWhenHidden = true;

    [Header("Internal")]
    bool trapFlag;
    float trapSpeedMultiplier = 1f;
    float trapControllerMultiplier = 1f;

    Renderer[] bodyRenderers;

    #region Unity Methods

    void Awake()
    {
        if (playerModel != null)
            bodyRenderers = playerModel.GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        MoveUpdate();
        LookUpdate();
        playerModel.transform.position = characterController.transform.position;
    }

    public override void OnStartClient()
    {
        // Make sure remote players don't run local input/camera
        if (!isLocalPlayer)
        {
            if (fpCamera != null)
            {
                fpCamera.enabled = false;

                var listener = fpCamera.GetComponentInChildren<AudioListener>();
                if (listener != null) listener.enabled = false;
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (hideBodyForOwner)
            SetLocalBodyHidden(true);
    }

    void SetLocalBodyHidden(bool hidden)
    {
        if (bodyRenderers == null || bodyRenderers.Length == 0)
        {
            if (playerModel != null)
                bodyRenderers = playerModel.GetComponentsInChildren<Renderer>(true);
            if (bodyRenderers == null) return;
        }

        foreach (var r in bodyRenderers)
        {
            if (r == null) continue;

            if (!hidden)
            {
                r.enabled = true;
                r.shadowCastingMode = ShadowCastingMode.On;
                continue;
            }

            if (keepShadowsWhenHidden)
            {
                r.enabled = true; // keep renderer enabled so it can cast shadows
                r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                r.enabled = false;
            }
        }
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
        Vector3 motion = transform.forward * MoveInput.y + transform.right * MoveInput.x * trapControllerMultiplier;
        motion.y = 0f;
        motion.Normalize();

        if (motion.sqrMagnitude >= 0.1f)
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, motion * MaxSpeed * trapSpeedMultiplier, Acceleration * Time.deltaTime);
        }
        else
        {
            CurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, Acceleration * Time.deltaTime);
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

        if (CurrentVelocity.sqrMagnitude > 0.01f)
        {
            PlayerAnimator.SetBool("IsRunning", true);
        }
        else
        {
            PlayerAnimator.SetBool("IsRunning", false);
        }
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(LookInput.x * LookSensitivity.x * trapControllerMultiplier, LookInput.y * LookSensitivity.y * trapControllerMultiplier);

        // Looking up and down
        CurrentPitch -= input.y;
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        // Looking left and right
        transform.Rotate(Vector3.up * input.x);
    }

    void HandleTrapEffects()
    {

    }

    #endregion
}
