using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Linq;

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
        PlayerCount.text = "Players Connected: "+n.ToString();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameObject settingsManager = Instantiate(NetworkManager.singleton.spawnPrefabs.SingleOrDefault(x => x.TryGetComponent<SettingsManager>(out _)));
        GameObject scoreManager = Instantiate(NetworkManager.singleton.spawnPrefabs.SingleOrDefault(x => x.TryGetComponent<ScoreManager>(out _)));
        NetworkServer.Spawn(settingsManager);
        NetworkServer.Spawn(scoreManager);
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
        Debug.Log("StartGameCalled: "+isServer);
        Debug.Log(SettingsManager.singleton.levelSelectMap["Maze"]);
        if (isServer)
        {
            FlyInBackground();
            ScoreManager.singleton.ResetGame();
            Invoke(nameof(AdvanceScene), 1.5f);
        }
    }

    void AdvanceScene()
    {
        SceneChanger.singleton.NewRandomScene();
    }

    [ClientRpc]
    void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }
}
