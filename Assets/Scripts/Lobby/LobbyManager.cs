using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField]
    public TMPro.TextMeshProUGUI Header;
    [SerializeField]
    public TMPro.TextMeshProUGUI PlayerCount;
    [SerializeField]
    public Button startButton;

    [SyncVar(hook = nameof(PlayerCountChanged))]
    int PlayerCountInt;
    void PlayerCountChanged(int _, int n)
    {
        PlayerCount.text = n.ToString();
    }

    void Start()
    {
        if (isServer)
        {
            PlayerCountInt = NetworkServer.connections.Count;
            Header.text = $"Hosting on {GetLocalIPAddress()}";
        }
        else
        {
            Header.text = "Joined";
            startButton.GetComponentInChildren<Text>().text = "Waiting for host";
            startButton.enabled = false;
        }
    }

    private void Update()
    {
        PlayerCountInt = NetworkServer.connections.Count;
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
        if (isServer)
        {
            FlyInBackground();
            Invoke(nameof(AdvanceScene), 1.5f);
        }
    }

    void AdvanceScene()
    {
        NetworkManager.singleton.ServerChangeScene("ColourMatch");
    }

    [ClientRpc]
    void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }
}
