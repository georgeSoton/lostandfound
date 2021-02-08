using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Random = UnityEngine.Random;


public class MatchGameManager : NetworkBehaviour
{
    string CorrectAnswer;
    string[] myAlpha;
    List<List<String>> GrantedExtraCharacters;
    Dictionary<int, float> wrongAnswerOptionsRates = new Dictionary<int, float> {
        {1, 0.5f},
        {2, 0.25f},
        {3, 0.2f},
        {5, 0.05f}
    };
    int wrongAnswerCount = 3;
    Minigame minigameType = Minigame.MatchGame;
    [SerializeField] FlexiSquareGridLayout TileGrid;
    [SerializeField] Local2DTile TilePrefab;
    List<Local2DTile> myTiles = new List<Local2DTile>();
    Dictionary<int, string> clientChoices = new Dictionary<int, string>();
    public override void OnStartServer()
    {
        base.OnStartServer();
        ScoreManager.singleton.StartMinigame();
        generateWrongAnswerCount();
        GrantedExtraCharacters = new List<List<string>>();
        myAlpha = Alphabets.Choices[Random.Range(0, Alphabets.Choices.Count)];
        CorrectAnswer = myAlpha[Random.Range(0, myAlpha.Length)];
        Debug.Log($"Correct answer is {CorrectAnswer}");
    }

    void generateWrongAnswerCount()
    {
        float selection = Random.Range(0f, 1f);
        float sum = wrongAnswerOptionsRates.Select(x => x.Value).Sum();
        float running = 0f;
        foreach (var entry in wrongAnswerOptionsRates)
        {
            running += entry.Value / sum;
            if (running > selection)
            {
                wrongAnswerCount = entry.Key;
                return;
            }
        }
        throw new InvalidOperationException("Randomly selecting the number of wrong answers broke.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdClientReady();
    }

    [Command(ignoreAuthority = true)]
    protected virtual void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        // We will use a local copy of the alphabet to select from
        var localAlpha = new List<String>(myAlpha);
        localAlpha.Remove(CorrectAnswer);
        // We have to make players 0 and 1 contain completely different options - otherwise
        // a two player game would have multiple "correct" answers.
        // Once we've made those two players different, we don't need to worry any more
        // because there can never be another option that appears to all players.
        if (GrantedExtraCharacters.Count == 1)
        {
            foreach (var forbidden in GrantedExtraCharacters[0]) { localAlpha.Remove(forbidden); }
        }

        var wrongs = new List<String>();
        for (var i = 0; i < wrongAnswerCount; i++)
        {
            var next = localAlpha[Random.Range(0, localAlpha.Count)];
            localAlpha.Remove(next);
            wrongs.Add(next);
        }
        GrantedExtraCharacters.Add(wrongs);

        var clientOptions = new List<string>(wrongs);
        clientOptions.Add(CorrectAnswer);
        ShuffleList(clientOptions);
        TargetSetOptions(conn, clientOptions);
    }

    [TargetRpc]
    protected virtual void TargetSetOptions(NetworkConnection conn, List<String> options)
    {
        var numOpts = options.Count;
        var sqrt = Mathf.CeilToInt(Mathf.Sqrt((float)numOpts));
        TileGrid.layouts.Clear();
        for (var i = 1; i < sqrt; i++)
        {
            if (numOpts % i == 0)
            {
                TileGrid.layouts.Add(new Vector2Int(i, numOpts / i));
                TileGrid.layouts.Add(new Vector2Int(numOpts / i, i));
            }
        }
        // Special case for preceisely square numbers
        if (sqrt * sqrt == numOpts) { TileGrid.layouts.Add(new Vector2Int(sqrt, sqrt)); }

        foreach (var option in options)
        {
            var parent = TileGrid.transform;
            var tile = Instantiate(TilePrefab);
            tile.Text = option;
            tile.transform.SetParent(parent);
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localScale = Vector3.one;
            myTiles.Add(tile);
            tile.Selected += TileClicked;
        }
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

    static void ShuffleList<T>(IList<T> inlist)
    {
        for (int i = 0; i < inlist.Count; i++)
        {
            T temp = inlist[i];
            int randomIndex = Random.Range(i, inlist.Count);
            inlist[i] = inlist[randomIndex];
            inlist[randomIndex] = temp;
        }
    }

    void TileClicked(string selection)
    {
        foreach (var tile in myTiles)
        {
            if (tile.Text == selection)
            {
                tile.SelectTile();
                CmdClientClicked(selection);
            }
            else
            {
                tile.DeselectTile();
            }
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdClientClicked(string option, NetworkConnectionToClient conn = null)
    {
        clientChoices[conn.connectionId] = option;

        if (NetworkServer.connections.All(x => clientChoices.ContainsKey(x.Value.connectionId) && clientChoices[x.Value.connectionId] == CorrectAnswer))
        {
            EndMinigame(true);
        }
    }

    void Destroy()
    {
        foreach (var tile in myTiles)
        {
            tile.Selected -= TileClicked;
        }
    }

}
