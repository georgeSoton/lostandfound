using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;

public class SortingManager : MinigameManagerBase
{
    [SerializeField] public List<GameObject> dice;
    [SerializeField] private List<ItemController> itemControllers;

    [SyncVar] int _top;
    [SyncVar] int _bot;

    private bool _taskFinished = false;

    private SyncList<int> _diceValues = new SyncList<int>();
    [SerializeField]
    public TMPro.TextMeshProUGUI topText;
    [SerializeField]
    public TMPro.TextMeshProUGUI bottomText;

    private void Awake()
    {
        itemControllers = new List<ItemController>();
        foreach (var die in dice)
        {
            itemControllers.Add(die.GetComponent<ItemController>());
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
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
    }


    [Command(ignoreAuthority = true)]
    protected override void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        }
        else
        {
            TargetMakeAssistant(conn, _top, _bot, dice);
        }
    }

    [TargetRpc]
    protected override void TargetMakePlayer(NetworkConnection conn)
    {
        base.TargetMakePlayer(conn);
        SetDiceValues(_diceValues.ToList());
    }

    private void SetDiceValues(List<int> diceValues)
    {
        if (diceValues.Count == itemControllers.Count)
        {
            for (int i = 0; i < itemControllers.Count; i++)
            {
                //Debug.Log(i + " " + diceValues[i]);
                itemControllers[i].SetValue(diceValues[i]);
            }
        }
    }

    [TargetRpc]
    private void TargetMakeAssistant(NetworkConnection conn, int top, int bot, List<GameObject> dice)
    {
        base.TargetMakeAssistant(conn);
        topText.text = top.ToString();
        bottomText.text = bot.ToString();
    }

    [Command(ignoreAuthority = true)]
    private void TrySolution(int sentTop, int sentBot)
    {
        if (_taskFinished)
        {
            return;
        }

        Debug.Log(sentTop + " " + sentBot);
        Debug.Log(_top + " " + _bot);


        if (sentTop == _top && sentBot == _bot)
        {
            _taskFinished = true;
            EndMinigame(true);
        }

        Debug.Log("Top:" + _top + "\nBot:" + _bot);
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


}