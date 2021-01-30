using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TransitionWipe : MonoBehaviour
{
    [SerializeField]
    RectTransform Bot;
    [SerializeField]
    RectTransform Mid;
    [SerializeField]
    RectTransform Top;

    [SerializeField]
    Camera BGCam;

    [SerializeField]
    List<Color> BGColours;

    static int BGindex = 0;

    bool isin = true;

    void Start()
    {
        BGCam.backgroundColor = BGColours[BGindex];

        Bot.localPosition = new Vector3(0, Bot.rect.height * -1.5f, 0);
        Mid.localPosition = new Vector3(0, Mid.rect.height * -1.5f, 0);
        Top.localPosition = new Vector3(0, Top.rect.height * -1.5f, 0);
    }
    public void Obscure()
    {
        var midcolour = BGColours[(BGindex + 2) % BGColours.Count];
        var topcolour = BGColours[(BGindex + 1) % BGColours.Count];
        BGindex = (BGindex + 1) % BGColours.Count;
        Mid.GetComponentInChildren<UnityEngine.UI.Image>().color = midcolour;
        Top.GetComponentInChildren<UnityEngine.UI.Image>().color = topcolour;

        Bot.DOLocalMoveY(0, 0.6f).SetEase(Ease.InOutCubic).Play();
        Mid.DOLocalMoveY(0, 0.8f).SetEase(Ease.InOutCubic).Play();
        Top.DOLocalMoveY(0, 1.2f).SetEase(Ease.InOutCubic).Play();
    }
}
