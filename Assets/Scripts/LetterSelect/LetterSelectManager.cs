using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Random = UnityEngine.Random;

public class LetterSelectManager : MinigameManagerBase
{
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

    bool subbedToTiles = false;
    bool TaskComplete = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
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
    }

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
        if (TaskComplete){return;}
        if (s == CorrectAnswer)
        {
            Debug.Log("Correct");
            TaskComplete = true;
            EndMinigame(true);
        }
        else
        {
            Debug.Log("Incorrect");
        }
    }

    [TargetRpc]
    protected override void TargetMakePlayer(NetworkConnection conn)
    {
        base.TargetMakePlayer(conn);
        SubToTiles();
    }

    [TargetRpc]
    protected override void TargetMakeAssistant(NetworkConnection conn)
    {
        base.TargetMakeAssistant(conn);
        UnsubFromTiles();
    }

    void OnDestroy()
    {
        UnsubFromTiles();
    }
}