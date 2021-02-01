using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LinkMaxFontSize : MonoBehaviour
{
    public TextMeshPro linkTmpro;
    TextMeshPro tmpro;
    // Start is called before the first frame update
    void Start()
    {
        tmpro = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpro.fontSizeMax = linkTmpro.fontSize;
    }
}
