using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class KatNetworkManager : NetworkManager
{
    public int playersToStart = 4;

    public Transform[] playerSpawnPoints;

    public GameObject Mask;
    public Transform[] maskSpawnPoints;

    private bool masksSpawned = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = playerSpawnPoints[numPlayers % playerSpawnPoints.Length];
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers == playersToStart && !masksSpawned)
        {
            // Spawn masks
            foreach (Transform maskSpawnPoint in maskSpawnPoints)
                SpawnMaskAtPosition(maskSpawnPoint);

            AssignRandomImposter();
            masksSpawned = true;
        }
    }

    void AssignRandomImposter()
    {
        int randomIndex = Random.Range(0, NetworkServer.connections.Count);
        int i = 0;

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (i == randomIndex)
            {
                conn.identity.GetComponent<Imposter>().isImposter = true;
                break;
            }
            i++;
        }
    }

    void SpawnMaskAtPosition(Transform trans)
    {
        GameObject mask = Instantiate(Mask, trans.position, Quaternion.identity);
        NetworkServer.Spawn(mask);
    }
}
