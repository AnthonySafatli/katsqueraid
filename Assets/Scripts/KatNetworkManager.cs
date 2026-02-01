using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class KatNetworkManager : NetworkManager
{
    public Transform[] playerSpawnPoints;
    public GameObject Mask;

    private bool maskSpawned = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = playerSpawnPoints[numPlayers % playerSpawnPoints.Length];
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers == 1 && !maskSpawned)
        {
            SpawnMask();
        }
    }

    void SpawnMask()
    {
        GameObject mask = Instantiate(Mask, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(mask);
        maskSpawned = true;
    }
}
