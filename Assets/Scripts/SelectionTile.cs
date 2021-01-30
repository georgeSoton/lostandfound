using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectionTile : NetworkBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI tm;
    [SyncVar(hook=nameof(ContentChangedHook))]
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

    void OnMouseDown()
    {
        Clicked.Invoke(Content);
    }

    public event System.Action<string> Clicked;
}