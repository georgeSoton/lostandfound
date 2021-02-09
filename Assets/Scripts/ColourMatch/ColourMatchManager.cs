using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ColourMatchManager : MinigameManagerBase
{
    [SerializeField]
    public UnityEngine.UI.Slider RSlider;
    [SerializeField]
    public UnityEngine.UI.Slider GSlider;
    [SerializeField]
    public UnityEngine.UI.Slider BSlider;
    [SerializeField]
    public UnityEngine.UI.Image PlayerColour;
    [SerializeField]
    public UnityEngine.UI.Image AssistantColour;
    [SerializeField]
    public float RequiredAccuracy = 10;

    bool taskfinished = false;

    [SyncVar]
    Color CorrectAnswer;
    [SyncVar(hook=nameof(TriedColourChanged))]
    Color TriedColour;
    private void Awake()
    {
        minigameType = Minigame.ColourMatch;
    }

    void TriedColourChanged(Color _, Color n)
    {
        //Debug.Log("New Colour = " + n.r + ", " + n.g + ", " + n.b);
        if (!amPlayer)
        {
            PlayerColour.color = n;
        }
    }
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        CorrectAnswer = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));

        var toTry = new Color(RSlider.value, GSlider.value, BSlider.value);
        PlayerColour.color = toTry;
        TriedColour = toTry;
    }

    [Command(ignoreAuthority=true)]
    protected override void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        } else
        {
            TargetMakeAssistant(conn, CorrectAnswer);
        }
    }

    [TargetRpc]
    void TargetMakeAssistant(NetworkConnection conn, Color correct)
    {
        base.TargetMakeAssistant(conn);
        AssistantColour.color = correct;
    }

    [Command(ignoreAuthority = true)]
    void TryColour(Color c)
    {
        if (taskfinished) {return;}
        TriedColour = c;
        PlayerColour.color = c;

        var dist = ColourDistance(c, CorrectAnswer);
        if (dist < RequiredAccuracy)
        {
            taskfinished = true;
            EndMinigame(true);
        }
        //Debug.Log(dist);
    }
    
    float ColourDistance(Color c1, Color c2)
    {
        return (Mathf.Pow(c1.r - c2.r,2)
        + Mathf.Pow(c1.g - c2.g,2)
        + Mathf.Pow(c1.b - c2.b,2));
    }
    
    public void SliderChanged()
    {
        //if (amPlayer) //for some reason this doesnt work, player doesnt call this apparently
        //{
        var toTry = new Color(RSlider.value, GSlider.value, BSlider.value);
        PlayerColour.color = toTry;
        TryColour(toTry);
        //}
    }

}
