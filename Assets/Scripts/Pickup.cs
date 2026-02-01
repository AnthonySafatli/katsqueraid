using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Pickup : NetworkBehaviour
{
    [Header("Pickup Settings")]
    public float pickupDistance = 3f;
    public Transform holdPoint;

    private GameObject heldObject;
    private PlayerInput playerInput;
    private InputAction grabAction;

    void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

    public override void OnStartLocalPlayer()
    {
        grabAction = playerInput.actions.FindAction("Grab", throwIfNotFound: true);
        playerInput.actions.Enable();
        grabAction.Enable();
        grabAction.performed += OnGrabPerformed;
    }

    void OnDestroy()
    {
        if (grabAction != null)
            grabAction.performed -= OnGrabPerformed;
    }

    void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        if (heldObject == null)
            CmdTryPickup();
        else
            CmdDrop();
    }

    [Command]
    void CmdTryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                heldObject = hit.collider.gameObject;
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();

                rb.isKinematic = true;
                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;

                RpcPickup(heldObject);
            }
        }
    }

    [Command]
    void CmdDrop()
    {
        if (heldObject == null) return;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        heldObject.transform.SetParent(null);
        rb.isKinematic = false;

        RpcDrop(heldObject);

        heldObject = null;
    }

    [ClientRpc]
    void RpcPickup(GameObject obj)
    {
        if (isLocalPlayer) return; // Local player already handled visuals
        heldObject = obj;
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }

    [ClientRpc]
    void RpcDrop(GameObject obj)
    {
        if (isLocalPlayer) return;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        obj.transform.SetParent(null);
        rb.isKinematic = false;
        heldObject = null;
    }

}
