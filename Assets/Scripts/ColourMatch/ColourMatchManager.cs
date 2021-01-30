using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ColourMatchManager : NetworkBehaviour
{
    bool amPlayer = false;

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
        if (!amPlayer)
        {
            PlayerColour.color = n;
        }
    }

    NetworkConnection playerID;

    public override void OnStartServer()
    {
        CorrectAnswer = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
        Debug.Log($"Correct answer is {CorrectAnswer}");
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
        var dist = ColourDistance(c, CorrectAnswer);
        if (dist < RequiredAccuracy)
        {
            taskfinished = true;
            NetworkManager.singleton.ServerChangeScene("ColourMatch");
        }
        Debug.Log(dist);
    }
    
    float ColourDistance(Color c1, Color c2)
    {
        return (Mathf.Pow(Mathf.Abs(c1.r - c2.r)/c2.r,2)
        + Mathf.Pow(Mathf.Abs(c1.g - c2.g)/c2.g,2)
        + Mathf.Pow(Mathf.Abs(c1.b - c2.b)/c2.b,2));
    }
    
    public void SliderChanged()
    {
        var totry = new Color(RSlider.value, GSlider.value, BSlider.value);
        PlayerColour.color = totry;
        TryColour(totry);
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
