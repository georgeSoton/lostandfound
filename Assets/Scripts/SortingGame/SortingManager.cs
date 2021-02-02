using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;

public class SortingManager : NetworkBehaviour
{
    bool _amPlayer = false;

    [FormerlySerializedAs("PlayerCam")] [SerializeField] public Camera playerCam;
    [FormerlySerializedAs("AssistantCam")] [SerializeField] public Camera assistantCam;
    [SyncVar][SerializeField] public List<GameObject> dice;
    [SyncVar] int _top;
    [SyncVar] int _bot;
    [SyncVar] private List<int> _diceValues;
    [SerializeField]
    public TMPro.TextMeshProUGUI topText;
    [SerializeField]
    public TMPro.TextMeshProUGUI bottomText;
    
    [SerializeField]

    NetworkConnection _playerID;

    private void Update()
    {
        if (isServer)
        {
            if (!NetworkServer.connections.ContainsKey(_playerID.connectionId))
            {
                Invoke(nameof(AdvanceScene), 1.5f);
            }
        }
    }

    public override void OnStartServer()
    {
        _diceValues = new List<int>();
        foreach (var die in dice)
        {
            var diceValue = Random.Range(1, 7);
            _diceValues.Add(diceValue);
        }
        var values = new List<int>(_diceValues);
        var tVal1 = values[Random.Range(0, values.Count)];
        values.Remove(tVal1);
        var tVal2 = values[Random.Range(0, values.Count)];
        values.Remove(tVal2);
        _top = tVal1 + tVal2;
        
        var bVal1 = values[Random.Range(0, values.Count)];
        values.Remove(bVal1);
        var bVal2 = values[Random.Range(0, values.Count)];
        values.Remove(bVal2);
        _bot = bVal1 + bVal2;
        
        Debug.Log($"Top answer is {_top}, Bot answer is {_bot}");
        var clientkeys = NetworkServer.connections.Keys.ToArray();
        _playerID = NetworkServer.connections[clientkeys[Random.Range(0, clientkeys.Length)]];

        //Resume timer
        ScoreManager.singleton.StartMinigame();
    }

    [Command(ignoreAuthority = true)]
    void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == _playerID.connectionId)
        {
            TargetMakePlayer(conn);
        }
        else
        {
            TargetMakeAssistant(conn, _top, _bot, dice);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdClientReady();
    }

    private bool _taskFinished = false;

    [Command(ignoreAuthority = true)]
    private void TrySolution(int sentTop, int sentBot)
    {
        if (_taskFinished)
        {
            return;
        }
        
        Debug.Log(sentTop+" "+sentBot);
        Debug.Log(_top+" "+_bot);
        

        if (sentTop == _top && sentBot == _bot)
        {
            _taskFinished = true;
            FlyInBackground();
            ScoreManager.singleton.MinigameComplete(Minigame.DiceSorting);
            Invoke(nameof(AdvanceScene), 1.5f);
        }

        Debug.Log("Top:" + _top + "\nBot:" + _bot);
    }

    private void AdvanceScene()
    {
        SceneChanger.singleton.NewRandomScene();
    }

    [ClientRpc]
    private void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }

    public void ValidateSolution()
    {
        var topSolution = 0; 
        var botSolution = 0;

        foreach (var dieData in dice.Select(die => die.GetComponent<ItemController>()))
        {
            switch (dieData.position)
            {
                case "top":
                    topSolution += dieData.diceValue;
                    break;
                case "bot":
                    botSolution += dieData.diceValue;
                    break;
            }
        }

        TrySolution(topSolution, botSolution);
    }

    [TargetRpc]
    private void TargetMakePlayer(NetworkConnection conn)
    {
        Debug.Log("Player");
        playerCam.gameObject.SetActive(true);
        playerCam.enabled = true;
        assistantCam.gameObject.SetActive(false);
        assistantCam.enabled = false;
        _amPlayer = true;
        for(int i =0;i<dice.Count;i++)
        {
            Debug.Log(i+" "+_diceValues[i]);
            dice[i].GetComponent<ItemController>().SetValue(_diceValues[i]);
        }
    }

    [TargetRpc]
    private void TargetMakeAssistant(NetworkConnection conn, int top, int bot, List<GameObject> dice)
    {
        Debug.Log("Assistant");
        playerCam.gameObject.SetActive(false);
        playerCam.enabled = false;
        assistantCam.gameObject.SetActive(true);
        assistantCam.enabled = true;
        _amPlayer = false;
        topText.text = top.ToString();
        bottomText.text = bot.ToString();
        
    }
}