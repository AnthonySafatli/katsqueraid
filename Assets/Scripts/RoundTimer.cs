using Mirror;
using UnityEngine;

public class RoundTimer : NetworkBehaviour
{
    [SyncVar] private double roundEndTime;

    public float roundDuration = 120f;

    public event System.Action OnRoundEnd;

    [Server]
    public void StartRound()
    {
        roundEndTime = NetworkTime.time + roundDuration;
    }

    void Update()
    {
        if (!isClient || roundEndTime == 0)
            return;

        if (NetworkTime.time >= roundEndTime)
        {
            roundEndTime = 0;
            OnRoundEnd?.Invoke();
        }
    }

    public float GetRemainingTime()
    {
        return roundEndTime == 0
            ? 0
            : (float)(roundEndTime - NetworkTime.time);
    }
}
