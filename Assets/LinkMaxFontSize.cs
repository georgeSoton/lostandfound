using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LinkMaxFontSize : MonoBehaviour
{
    public TextMeshProUGUI linkTmpro;
    TextMeshProUGUI tmpro;
    // Start is called before the first frame update
    void Awake()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpro.fontSizeMax = linkTmpro.fontSize;
    }
}
