using Mirror;
using UnityEngine;

public class MaskSharedState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnValueChanged))]
    public bool isGrabbed;

    void OnValueChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"Mask grabbed state changed from {oldValue} to {newValue}");
    }

    [Command(requiresAuthority = false)]
    void CmdSetValue(bool newValue)
    {
        isGrabbed = newValue;
    }

    [Client]
    public void RequestChange(bool newValue)
    {
        CmdSetValue(newValue);
    }
}