using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Random = UnityEngine.Random;

public class LetterSelectManager : NetworkBehaviour
{
    [SerializeField] public Camera PlayerCam;
    [SerializeField] public Camera AssistantCam;
    [SerializeField] public List<SelectionTile> PlayerTiles;
    [SerializeField] public SelectionTile AssistantClue;

    List<String[]> Alphabets = new List<String[]>
    {
        new string[] {":)", ":(", ";)", ";P", ":o", ">:(", ":c", "¬_¬", "uwu", ":'(", ":')", ">_<", "O-o", ":D", ">:D", ":S", ":X", ":|", ":/", ";*", "xD", "=3", "( ͡° ͜ʖ ͡°)", "T-T"},
        new string[] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
        "W", "X", "Y", "Z"},
        new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"},
        new string[] {"!", "?", "@", "%", "£", "$", "*", ":", ";", "(", "{", "<", "&", "\"","#", "+", "-", "=", "^"}
    };
    string CorrectAnswer = null;

    NetworkConnection playerID;

    public override void OnStartServer()
    {
        ScoreManager.singleton.StartMinigame();
        
        var localalpha = new List<string>(Alphabets[Random.Range(0, Alphabets.Count)]);
        CorrectAnswer = localalpha[Random.Range(0, localalpha.Count)];
        Debug.Log($"Correct answer is {CorrectAnswer}");
        localalpha.Remove(CorrectAnswer);
        AssistantClue.SetText(CorrectAnswer);
        foreach (var tile in PlayerTiles)
        {
            var nextoption = localalpha[Random.Range(0, localalpha.Count)];
            tile.SetText(nextoption);
            localalpha.Remove(nextoption);
        }

        PlayerTiles[Random.Range(0, PlayerTiles.Count)].SetText(CorrectAnswer);

        var clientkeys = NetworkServer.connections.Keys.ToArray();
        playerID = NetworkServer.connections[clientkeys[Random.Range(0, clientkeys.Length)]];
    }

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

    [Command(ignoreAuthority = true)]
    void CmdClientReady(NetworkConnectionToClient conn = null)
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdClientReady();
    }

    bool subbedToTiles = false;

    void SubToTiles()
    {
        Debug.Log("Subbing to tiles");
        if (!subbedToTiles)
        {
            foreach (var tile in PlayerTiles)
            {
                tile.Clicked += TileClickedHandler;
            }
        }
    }

    void UnsubFromTiles()
    {
        Debug.Log("Unsubbing to tiles");
        if (subbedToTiles)
        {
            foreach (var tile in PlayerTiles)
            {
                tile.Clicked -= TileClickedHandler;
            }
        }
    }

    void TileClickedHandler(string s)
    {
        CmdPlayerChose(s);
    }

    bool TaskComplete = false;
    [Command(ignoreAuthority = true)]
    void CmdPlayerChose(string s)
    {
        if (TaskComplete){return;}
        if (s == CorrectAnswer)
        {
            Debug.Log("Correct");
            TaskComplete = true;
            FlyInBackground();
            ScoreManager.singleton.MinigameComplete(Minigame.LetterSelect);
            Invoke(nameof(AdvanceScene), 1.5f);
        }
        else
        {
            Debug.Log("Incorrect");
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

    [TargetRpc]
    void TargetMakePlayer(NetworkConnection conn)
    {
        Debug.Log("Player");
        PlayerCam.gameObject.SetActive(true);
        PlayerCam.enabled = true;
        AssistantCam.gameObject.SetActive(false);
        AssistantCam.enabled = false;
        SubToTiles();
    }

    [TargetRpc]
    void TargetMakeAssistant(NetworkConnection conn)
    {
        Debug.Log("Assistant");
        PlayerCam.gameObject.SetActive(false);
        PlayerCam.enabled = false;
        AssistantCam.gameObject.SetActive(true);
        AssistantCam.enabled = true;
        UnsubFromTiles();
    }

    void OnDestroy()
    {
        UnsubFromTiles();
    }
}