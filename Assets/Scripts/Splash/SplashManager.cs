using UnityEngine;
using DG.Tweening;
using Mirror;

public class SplashManager : NetworkBehaviour
{
    [SerializeField] RectTransform[] FromLeftElements;
    [SerializeField] RectTransform[] FromRightElements;
    [SerializeField] RectTransform LeftFromTarget;
    [SerializeField] RectTransform RightFromTarget;

    [SerializeField] float inTimeMin = 0.7f;
    [SerializeField] float inTimeMax = 0.9f;
    [SerializeField] float midTimeMin = 2.5f;
    [SerializeField] float midTimeMax = 2.65f;
    [SerializeField] float outtimeMin = 0.7f;
    [SerializeField] float outtimeMax = 0.9f;
    [Scene]
    [SerializeField] string NextScene;

    void Awake()
    {
        foreach (var el in FromLeftElements)
        {
            el.GetComponent<TMPro.TextMeshProUGUI>().enabled = false;
        }
        foreach (var el in FromRightElements)
        {
            el.GetComponent<TMPro.TextMeshProUGUI>().enabled = false;
        }
    }

    public override void OnStartServer()
    {
        Invoke(nameof(AdvanceScene), inTimeMax + midTimeMax + outtimeMax + 0.3f);
    }

    public override void OnStartClient()
    {
        Debug.Log("SplashManager client start");
        foreach (var el in FromLeftElements)
        {
            TweenInOut(el, LeftFromTarget, RightFromTarget);
        }
        foreach (var el in FromRightElements)
        {
            TweenInOut(el, RightFromTarget, LeftFromTarget);
        }
    }

    void TweenInOut(RectTransform el, RectTransform from, RectTransform to)
    {
        el.GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
        var origpos = el.localPosition.x;
        el.localPosition = new Vector3(from.localPosition.x, el.localPosition.y, el.localPosition.z);

        var sequence = DOTween.Sequence();
        sequence.Append(el.DOLocalMoveX(origpos, Random.Range(inTimeMin, inTimeMax)).SetRelative(false).SetEase(Ease.OutSine));
        sequence.AppendInterval(Random.Range(midTimeMin, midTimeMax));
        sequence.Append(el.DOLocalMoveX(to.localPosition.x, Random.Range(outtimeMin, outtimeMax)).SetRelative(false).SetEase(Ease.InSine));
        sequence.Play();
    }
    [Server]
    void AdvanceScene()
    {
        NetworkManager.singleton.ServerChangeScene(NextScene);
    }
}
