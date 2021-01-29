using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class NetworkText : NetworkBehaviour
{
    [SyncVar(hook = "TextChanged")]
    string DisplayText;

    void TextChanged(string o, string n)
    {
        mytext.text = n;
    }

    [SerializeField]
    TextMeshPro mytext;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int count = 0;

    [Server]
    public void Increment()
    {
        count += 1;
        DisplayText = $"Count={count}";
    }
}
