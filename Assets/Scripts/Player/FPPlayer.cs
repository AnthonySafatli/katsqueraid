using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
[RequireComponent(typeof(PlayerInput))]
public class FPPlayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] FPController FPController;
    [SerializeField] PlayerInput playerInput;

    #region Input Handling

    void Awake()
    {
        FPController.GetComponent<FPController>();
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnStartClient()
    {
        if (playerInput != null)
            playerInput.enabled = true;

        enabled = isLocalPlayer;
    }

    public override void OnStartLocalPlayer()
    {
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.ActivateInput();
        }
    }

    void OnMove(InputValue value) => FPController.MoveInput = value.Get<Vector2>();

    void OnLook(InputValue value) => FPController.LookInput = value.Get<Vector2>();

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            FPController.TryJump();
        }
    }

    #endregion

    #region Unity Methods

    // void OnValidate()
    // {
    //     if (FPController == null)
    //         FPController = GetComponent<FPController>();
    // }

    #endregion
}
