using System;
using Mirror;

public abstract class Trap : NetworkBehaviour
{
    public abstract String TrapType { get; }
    public abstract float Duration { get; }
}
