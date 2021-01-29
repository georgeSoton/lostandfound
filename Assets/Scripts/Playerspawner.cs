using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Playerspawner : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject prefab;
    void Start()
    {
        if (isServer)
        {
            foreach (var clientid in NetworkServer.connections)
            {
                var obj = Instantiate(prefab);;
                NetworkServer.RemovePlayerForConnection(clientid.Value, true);
                NetworkServer.AddPlayerForConnection(clientid.Value, obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
