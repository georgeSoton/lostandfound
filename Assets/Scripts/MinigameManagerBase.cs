using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public abstract class MinigameManagerBase : NetworkBehaviour
{
    
    [SerializeField] public Camera PlayerCam;
    [SerializeField] public Camera AssistantCam;
    [SerializeField] public List<SelectionTile> PlayerTiles;
    [SerializeField] public SelectionTile AssistantClue;

    protected NetworkConnection playerID;

    protected bool amPlayer = false;
    protected Minigame minigameType = Minigame.None;

    private void Update()
    {
        if (isServer)
        {
            if (!NetworkServer.connections.ContainsKey(playerID.connectionId))
            {
                Invoke(nameof(AdvanceScene), 1.5f);
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        var clientkeys = NetworkServer.connections.Keys.ToArray();
        playerID = NetworkServer.connections[clientkeys[Random.Range(0, clientkeys.Length)]];
        ScoreManager.singleton.StartMinigame();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdClientReady();
    }

    [Command(ignoreAuthority = true)]
    protected virtual void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        }
        else
        {
            TargetMakeAssistant(conn);
        }
    }

    [TargetRpc]
    protected virtual void TargetMakePlayer(NetworkConnection conn)
    {
        Debug.Log("Player");
        PlayerCam.gameObject.SetActive(true);
        PlayerCam.enabled = true;
        AssistantCam.gameObject.SetActive(false);
        AssistantCam.enabled = false;
        amPlayer = true;
    }

    [TargetRpc]
    protected virtual void TargetMakeAssistant(NetworkConnection conn)
    {
        Debug.Log("Assistant");
        PlayerCam.gameObject.SetActive(false);
        PlayerCam.enabled = false;
        AssistantCam.gameObject.SetActive(true);
        AssistantCam.enabled = true;
        amPlayer = true;
    }

    [ClientRpc]
    void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }

    [Server]
    protected void EndMinigame(bool isWin)
    {
        FlyInBackground();
        if (isWin) //level complete
        {
            ScoreManager.singleton.MinigameComplete(minigameType);
            Invoke(nameof(AdvanceScene), 1.5f);
        }
        else //TimeOut or player disconnect
        {
            Debug.LogWarning("TODO: NOT YET IMPLEMENTED");
        }
    }

    [Server]
    private void AdvanceScene()
    {
        SceneChanger.singleton.NewRandomScene();
    }

}
