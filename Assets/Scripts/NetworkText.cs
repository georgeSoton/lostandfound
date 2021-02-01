using UnityEngine;

public class NetworkText : MonoBehaviour
{
    TMPro.TextMeshProUGUI tm;

    public void Awake()
    {
        tm = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void SetText(string txt)
    {
        tm.text = txt;
    }
}