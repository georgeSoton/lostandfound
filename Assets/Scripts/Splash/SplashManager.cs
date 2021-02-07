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

    void Start()
    {
        foreach (var el in FromLeftElements)
        {
            el.gameObject.SetActive(false);
        }
        foreach (var el in FromRightElements)
        {
            el.gameObject.SetActive(false);
        }
    }

    public override void OnStartServer()
    {
        Invoke(nameof(AdvanceScene), inTimeMax + midTimeMax + outtimeMax + 0.3f);
    }

    public override void OnStartClient()
    {
        foreach (var el in FromLeftElements)
        {
            TweenInOut(el, LeftFromTarget, RightFromTarget);
        }
        foreach (var el in FromRightElements)
        {
            TweenInOut(el, RightFromTarget, LeftFromTarget);
        }
        Invoke(nameof(AdvanceScene), inTimeMax + midTimeMax + outtimeMax);
    }

    void TweenInOut(RectTransform el, RectTransform from, RectTransform to)
    {
        el.gameObject.SetActive(true);
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
        Debug.Log("Advancing Scene lets gooooooooooooooo");
        NetworkManager.singleton.ServerChangeScene(NextScene);
    }
}
