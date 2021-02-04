using UnityEngine;

public class ScoreSync : MonoBehaviour
{
    NetworkText text;

    private void Awake()
    {
        text = GetComponent<NetworkText>();
    }

    private void Update()
    {
        text.SetText(ScoreManager.singleton.Score.ToString());
    }
}
