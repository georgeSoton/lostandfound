using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class SummaryManager : NetworkBehaviour
{
    [SerializeField]
    Button lobbyBtn;
    [SerializeField]
    TextMeshProUGUI lobbyBtnText;
    
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
        lobbyBtn.onClick.AddListener(ReturnToLobbyTrigger);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        lobbyBtn.enabled = false;
        lobbyBtnText.text = "Waiting for Host...";
    }

    private void Start()
    {
        //TODO: Slide in results
    }
    

    void RevealRating()
    {

    }

    void RevealRoundsPassed()
    {

    }

    void RevealScore()
    {

    }

    public void ReturnToLobbyTrigger()
    {
        SceneChanger.singleton.ReturnToLobby();
    }

}
