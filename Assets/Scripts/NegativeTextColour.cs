using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeTextColour : MonoBehaviour
{
    public UnityEngine.UI.Image syncImg;
    TMPro.TextMeshProUGUI tmpro;
    
    // Start is called before the first frame update
    void Start()
    {
        tmpro = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpro.color = new Color(1 - syncImg.color.r, 1 - syncImg.color.g, 1 - syncImg.color.b, 1);
        //tmpro.SetMaterialDirty();
    }
}
