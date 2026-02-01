using Mirror;
using UnityEngine;


[AddComponentMenu("")]
public class KatNetworkManager : NetworkManager
{
    public Transform[] playerSpawnPoints;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = playerSpawnPoints[numPlayers % playerSpawnPoints.Length];
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // TODO Custom logic before player is removed can be added here
        base.OnServerDisconnect(conn);
    }
}
