using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TimerSync : NetworkBehaviour
{
    public NetworkText text;

    private void Update()
    {
        if (isServer)
        {
            text.SetText(ScoreManager.singleton.GetTimeRemaining());
        }
    }
}
