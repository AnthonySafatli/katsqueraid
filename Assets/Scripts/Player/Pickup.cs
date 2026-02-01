using Mirror;
using Mirror.Examples.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickup : NetworkBehaviour
{
    [Header("Pickup Settings")]
    public float pickupDistance = 3f;
    public Transform holdPoint;

    private GameObject heldObject;
    private PlayerInput playerInput;
    private InputAction grabAction;

    private bool wasGrabPressed;

    void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            if (playerInput != null) playerInput.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (playerInput != null) playerInput.enabled = true;

        grabAction = playerInput.actions.FindAction("Grab", throwIfNotFound: true);
    }

    void Update()
    {
        if (!isLocalPlayer || grabAction == null) return;

        bool grabPressed = grabAction.ReadValue<float>() > 0.5f;

        if (grabPressed)
        {
            if (heldObject == null && !wasGrabPressed)
            {
                TryPickup(); 
            }
        }
        else
        {
            if (heldObject != null)
            {
                Drop();
            }
        }

        wasGrabPressed = grabPressed;

        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = holdPoint.rotation;
        }
    }


    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                var hitObj = hit.collider.gameObject;
                var state = hitObj.GetComponent<MaskSharedState>();

                if (state == null || state.isGrabbed) return;
                
                state.RequestChange(true);
                heldObject = hitObj;
            }
        }
    }

    void Drop()
    {
        if (heldObject == null) return;

        heldObject.GetComponent<MaskSharedState>().RequestChange(false);
        heldObject = null;
    }

    public int GetMaskValue()
    {
        if (heldObject == null) return 0;
        return 1; // TODO: heldObject.GetComponent<MaskSharedState>().maskValue;
    }

    public void DropZoneDrop()
    {
        heldObject.GetComponent<MaskSharedState>().RequestChange(false);
        NetworkServer.Destroy(heldObject);
        heldObject = null;
    }
}
