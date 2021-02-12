using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultNumberBehaviour : MonoBehaviour
{
    [SerializeField][Range(0,3)]
    float revealTimeSeconds;
    [SerializeField]
    string preambleText;
    [SerializeField]
    TMPro.TextMeshProUGUI tm;

    float result;
    bool isReveal;
    float revealTimeElapsed;
    public void Reveal(int result)
    {
        this.result = result;
        isReveal = true;
        revealTimeElapsed = 0;
    }

    private void Update()
    {
        if (isReveal)
        {
            revealTimeElapsed += Time.deltaTime;
            float t = Mathf.Clamp(revealTimeElapsed / revealTimeSeconds, 0, 1);
            tm.text = preambleText +"\n"+ string.Format("{0:0}", Mathf.SmoothStep(0, result, t));
            if(t >= 1)
            {
                isReveal = false;
            }
        }
    }


}
