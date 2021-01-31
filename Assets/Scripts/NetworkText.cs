using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkText : NetworkBehaviour
{
    [SerializeField]
    TMPro.TextMeshPro tm;
    [SyncVar(hook = nameof(ContentChangedHook))]
    string Content;

    void ContentChangedHook(string _, string n)
    {
        tm.text = n;
    }

    [Server]
    public void SetText(string txt)
    {
        Content = txt;
    }
}