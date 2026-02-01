using Mirror;
using UnityEngine;

public class Imposter : NetworkBehaviour
{
    [SyncVar]
    public bool isImposter = false;
}
