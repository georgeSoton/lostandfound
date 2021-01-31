using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
public class SelectionTile : NetworkBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI tm;
    [SyncVar(hook=nameof(ContentChangedHook))]
    string Content;
    Vector3 targetscale;
    void Awake()
    {
        targetscale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    void Start()
    {
        transform.DOScale(targetscale, Random.Range(0.5f, 1f)).Play();
    }

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
        if (Clicked!=null) {Clicked.Invoke(Content);}
    }

    public event System.Action<string> Clicked;
}