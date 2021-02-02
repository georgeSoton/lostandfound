using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ColourMatchManager : NetworkBehaviour
{
    public bool amPlayer = false;

    [SerializeField]
    public Camera PlayerCam;
    [SerializeField]
    public Camera AssistantCam;
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
    [SyncVar]
    Color CorrectAnswer;

    [SyncVar(hook=nameof(TriedColourChanged))]
    Color TriedColour;
    void TriedColourChanged(Color _, Color n)
    {
        //Debug.Log("New Colour = " + n.r + ", " + n.g + ", " + n.b);
        if (!amPlayer)
        {
            PlayerColour.color = n;
        }
    }

    NetworkConnection playerID;

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
        ScoreManager.singleton.StartMinigame();
        CorrectAnswer = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
        Debug.Log($"Correct answer is {CorrectAnswer}");
        var clientkeys = NetworkServer.connections.Keys.ToArray();
        playerID = NetworkServer.connections[clientkeys[Random.Range(0, clientkeys.Length)]];

        var toTry = new Color(RSlider.value, GSlider.value, BSlider.value);
        PlayerColour.color = toTry;
        TriedColour = toTry;
    }

    [Command(ignoreAuthority=true)]
    void CmdClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        } else
        {
            TargetMakeAssistant(conn, CorrectAnswer);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdClientReady();
    }

    bool taskfinished = false;
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
            FlyInBackground();
            ScoreManager.singleton.MinigameComplete(Minigame.ColourMatch);
            Invoke(nameof(AdvanceScene), 1.5f);
        }
        //Debug.Log(dist);
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

    [TargetRpc]
    void TargetMakePlayer(NetworkConnection conn)
    {
        Debug.Log("Player");
        PlayerCam.gameObject.SetActive(true);
        PlayerCam.enabled = true;
        AssistantCam.gameObject.SetActive(false);
        AssistantCam.enabled = false;
        amPlayer = true;
    }
    [TargetRpc]
    void TargetMakeAssistant(NetworkConnection conn, Color correct)
    {
        Debug.Log("Assistant");
        PlayerCam.gameObject.SetActive(false);
        PlayerCam.enabled = false;
        AssistantCam.gameObject.SetActive(true);
        AssistantCam.enabled = true;
        amPlayer = false;
        AssistantColour.color = correct;
    }
}
