using Mirror;

public class ScoreSync : NetworkBehaviour
{
    public NetworkText text;

    private void Update()
    {
        if (isServer)
        {
            text.SetText(ScoreManager.singleton.Score.ToString());
        }
    }
}
