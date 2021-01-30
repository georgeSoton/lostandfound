using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField]
    public TMPro.TextMeshProUGUI Header;
    [SerializeField]
    public TMPro.TextMeshProUGUI PlayerCount;

    [SyncVar(hook=nameof(PlayerCountChanged))]
    int PlayerCountInt;
    void PlayerCountChanged(int _, int n)
    {
        PlayerCount.text = n.ToString();
    }

    void Update()
    {
        if (isServer)
        {
            PlayerCountInt = NetworkServer.connections.Count;
            Header.text = $"Hosting on {GetLocalIPAddress()}";
        } else {
            Header.text = "Joined";
        }
    }

    public string GetLocalIPAddress()
     {
         var host = Dns.GetHostEntry(Dns.GetHostName());
         foreach (var ip in host.AddressList)
         {
             if (ip.AddressFamily == AddressFamily.InterNetwork)
             {
                 return ip.ToString();
             }
         }
         throw new System.Exception("No network adapters with an IPv4 address in the system!");
     }

     public void StartGame()
     {
         NetworkManager.singleton.ServerChangeScene("ColourMatch");
     }
}
