using System;
using System.Collections;
using Mirror;
using UnityEngine;

public abstract class Trap : NetworkBehaviour
{
    public abstract String TrapType { get; }
    public abstract float Duration { get; }
    public IEnumerator ActivateTrap(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
}
