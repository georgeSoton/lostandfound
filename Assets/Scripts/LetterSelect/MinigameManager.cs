using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class MinigameManager : NetworkBehaviour
{

    [SerializeField]
    public Camera PlayerCam;
    [SerializeField]
    public Camera AssistantCam;
    [SerializeField]
    public List<SelectionTile> PlayerTiles;
    [SerializeField]
    public SelectionTile AssistantClue;

    static string[] Alphabet = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    string CorrectAnswer = null;

    NetworkConnection playerID;

    public override void OnStartServer()
    {
        var localalpha = new List<string>(Alphabet);
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

    [Command(ignoreAuthority=true)]
    void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        } else
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

    [Command(ignoreAuthority = true)]
    void CmdPlayerChose(string s)
    {
        if (s == CorrectAnswer)
        {
            Debug.Log("Correct");
            NetworkManager.singleton.ServerChangeScene("LetterSelect");
        }
        else
        {
            Debug.Log("Incorrect");
        }
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
