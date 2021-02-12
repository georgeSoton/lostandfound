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
    [SerializeField]
    ResultNumberBehaviour score;
    [SerializeField]
    ResultNumberBehaviour roundCount;
    [SerializeField]
    RatingBehaviour rating;
    
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClientOnly)
        {
            lobbyBtn.interactable = false;
            lobbyBtnText.text = "Waiting for Host...";
        }
        else
        {
            lobbyBtn.interactable = true;
        }
    }

    private void Start()
    {
        Invoke(nameof(RevealRoundsPassed), 1f);
        Invoke(nameof(RevealScore), 1.5f);
        Invoke(nameof(RevealRating), 2f);
    }
    
    void RevealRating()
    {
        int starCount = 0;
        if (ScoreManager.singleton.Score >= 2000)
        {
            starCount += 1;
        }
        if (ScoreManager.singleton.Score >= 5000)
        {
            starCount += 1;
        }
        if (ScoreManager.singleton.Score >= 10000)
        {
            starCount += 1;
        }

        rating.ShowStars(starCount);
    }

    void RevealRoundsPassed()
    {
        roundCount.Reveal(ScoreManager.singleton.RoundCount);
    }

    void RevealScore()
    {
        score.Reveal(ScoreManager.singleton.Score);
    }

    public void ReturnToLobbyTrigger()
    {
        Debug.Log("Returning to lobby");
        if (!isClientOnly)
        {
            FlyInBackground();
            Invoke(nameof(AdvanceScene),1.5f);
        }
    }

    private void AdvanceScene()
    {
        SceneChanger.singleton.ReturnToLobby();
    }

    [ClientRpc]
    void FlyInBackground()
    {
        FindObjectOfType<TransitionWipe>().Obscure();
    }

}
