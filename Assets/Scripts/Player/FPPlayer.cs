using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class FPPLayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] FPController FPController;

    #region Input Handling

    void OnMove(InputValue value)
    {
        if (!isLocalPlayer) return;

        FPController.MoveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        if (!isLocalPlayer) return;

        FPController.LookInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isLocalPlayer) return;

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
