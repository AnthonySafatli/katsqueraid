using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTotalPointsChanged))]
    public int totalPoints;

    [SyncVar(hook = nameof(OnImposterPointsChanged))]
    public int imposterPoints;

    [SerializeField] TextMeshProUGUI pointText;

    void OnTotalPointsChanged(int oldValue, int newValue)
    {
        Debug.Log($"Total Points changed: {oldValue} -> {newValue}");
    }

    void OnImposterPointsChanged(int oldValue, int newValue)
    {
        Debug.Log($"Imposter Points changed: {oldValue} -> {newValue}");
    }

    [Command(requiresAuthority = false)]
    void CmdSetValues(int total, int imposter)
    {
        totalPoints = total;
        imposterPoints = imposter;
    }

    [Client]
    public void RequestChange(int total, int imposter)
    {
        CmdSetValues(total, imposter);
    }
}
