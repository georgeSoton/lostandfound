using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;
public abstract class MinigameManagerBase : NetworkBehaviour
{
    [SerializeField] public Camera PlayerCam;
    [SerializeField] public Camera AssistantCam;
    [SerializeField] TextMeshProUGUI playerIndicator;
    protected NetworkConnection playerID;
    [SerializeField]
    protected bool amPlayer = false;
    protected Minigame minigameType = Minigame.None;
    
    private bool endGame;

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
        endGame = false;
        ScoreManager.singleton.OnEndGame += EndGame;
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
        if(!ReferenceEquals(PlayerCam, AssistantCam))
        {
            AssistantCam.gameObject.SetActive(false);
            AssistantCam.enabled = false;
        }
        amPlayer = true;
        SetPlayerIndicator();
    }

    [TargetRpc]
    protected virtual void TargetMakeAssistant(NetworkConnection conn)
    {
        Debug.Log("Assistant");
        if (!ReferenceEquals(PlayerCam, AssistantCam))
        {
            PlayerCam.gameObject.SetActive(false);
            PlayerCam.enabled = false;
        }
        AssistantCam.gameObject.SetActive(true);
        AssistantCam.enabled = true;
        amPlayer = false;
        SetPlayerIndicator();
    }

    [ClientRpc]
    void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }

    [Server]
    protected void EndMinigame(bool isWin)
    {
        if (!IsInvoking(nameof(AdvanceScene)))
        {
            ScoreManager.singleton.OnEndGame -= EndGame;
            FlyInBackground();
            if (isWin) //level complete
            {
                ScoreManager.singleton.MinigameComplete(minigameType);
            }
            Invoke(nameof(AdvanceScene), 1.5f);
        }
    }

    [Server]
    private void AdvanceScene()
    {
        if (endGame)
        {
            SceneChanger.singleton.EndGame();
        }
        else
        {
            SceneChanger.singleton.NewRandomScene();
        }
    }

    

    protected void ShuffleList<T>(IList<T> inlist)
    {
        for (int i = 0; i < inlist.Count; i++)
        {
            T temp = inlist[i];
            int randomIndex = Random.Range(i, inlist.Count);
            inlist[i] = inlist[randomIndex];
            inlist[randomIndex] = temp;
        }
    }

    void SetPlayerIndicator()
    {
        if (playerIndicator != null)
        {
            if (amPlayer)
            {
                playerIndicator.text = "Player";
            }
            else
            {
                playerIndicator.text = "Assistant";
            }
        }
    }


    private void EndGame()
    {
        endGame = true;
        EndMinigame(false);
    }

    void OnDestroy()
    {
        ScoreManager.singleton.OnEndGame -= EndGame;
    }
}
