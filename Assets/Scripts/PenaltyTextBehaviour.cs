using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyTextBehaviour : MonoBehaviour
{
    [SerializeField]
    float fadeTimeSeconds;
    TMPro.TextMeshProUGUI tm;
    Color setColor, transparentColor;
    bool fadeOut;
    bool isInit;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        isInit = false;
        Init();
    }

    private void Init()
    {
        if (ScoreManager.singleton != null)
        {
            ScoreManager.singleton.OnPenalty += DisplayPenalty;
            tm = GetComponent<TMPro.TextMeshProUGUI>();
            setColor = new Color(tm.color.r, tm.color.g, tm.color.b, 1);
            transparentColor = new Color(tm.color.r, tm.color.g, tm.color.b, 0);
            tm.color = transparentColor;
            t = 0;
            isInit = true;
        }
        else
        {
            isInit = false;
        }
    }

    void DisplayPenalty(float penalty)
    {
        tm.text = "-" + penalty + "s";
        tm.color = setColor;
        fadeOut = true;
        t = 0;
    }

    private void Update()
    {
        if (!isInit) Init();
        if (fadeOut)
        {
            t += Time.deltaTime/fadeTimeSeconds;
            tm.color = Color.Lerp(setColor, transparentColor, t);
            if (t >= 1)
            {
                fadeOut = false;
                t = 0;
            }
        }
    }

    
    private void OnDestroy()
    {
        ScoreManager.singleton.OnPenalty -= DisplayPenalty;
        //Debug.Log("DESTROYING PENALTY TEXT BEHAVIOUR");
    }
}
