using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropZone : MonoBehaviour
{
    public GameObject dropOffUi;
    public GameObject stealUi;

    public GameManager gameManager;

    private PlayerInput playerInput;
    private InputAction dropOffAction;
    private InputAction stealAction;

    void OnTriggerEnter(Collider other)
    {
        if (NetworkClient.localPlayer.gameObject != other.gameObject) return;

        bool isImposter = NetworkClient.localPlayer.GetComponent<Imposter>().isImposter;

        if (dropOffUi != null) 
            dropOffUi.SetActive(true);

        if (stealUi != null && isImposter)
            stealUi.SetActive(true);

        playerInput = other.GetComponent<PlayerInput>();

        dropOffAction = playerInput.actions["DropOff"];
        dropOffAction.performed += OnPlayerDrop;

        if (isImposter)
        {
            stealAction = playerInput.actions["Steal"];
            stealAction.performed += OnPlayerSteal;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (NetworkClient.localPlayer.gameObject != other.gameObject) return;

        if (dropOffUi != null) 
            dropOffUi.SetActive(false);

        if (stealUi != null)
            stealUi.SetActive(false);

        dropOffAction.performed -= OnPlayerDrop;
        dropOffAction = null;

        stealAction.performed -= OnPlayerSteal;
        stealAction = null;

        playerInput = null;
    }

    private void OnPlayerDrop(InputAction.CallbackContext context)
    {
        var pickupScript = NetworkClient.localPlayer.GetComponentInChildren<Pickup>();

        var maskValue = pickupScript.GetMaskValue();
        if (maskValue == 0) return;

        gameManager.RequestChange(gameManager.totalPoints + maskValue, gameManager.imposterPoints);
        pickupScript?.DropZoneDrop();
    }

    private void OnPlayerSteal(InputAction.CallbackContext context)
    {
        if (!NetworkClient.localPlayer.GetComponent<Imposter>().isImposter) return;

        var pickupScript = NetworkClient.localPlayer.GetComponentInChildren<Pickup>();

        var maskValue = pickupScript.GetMaskValue();
        if (maskValue == 0) return;

        gameManager.RequestChange(gameManager.totalPoints, gameManager.imposterPoints + maskValue);
        pickupScript?.DropZoneDrop();
    }
}
