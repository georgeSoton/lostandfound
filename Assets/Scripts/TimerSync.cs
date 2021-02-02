using UnityEngine;

public class TimerSync : MonoBehaviour
{
    NetworkText text;

    private void Awake()
    {
        text = GetComponent<NetworkText>();
    }

    private void Update()
    {
        text.SetText(ScoreManager.singleton.GetTimeRemaining());
    }
}
