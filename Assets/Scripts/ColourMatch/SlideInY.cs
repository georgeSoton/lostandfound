using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SlideInY : MonoBehaviour
{
    [SerializeField]
    public float Duration = 0.8f;
    // Start is called before the first frame update

    [SerializeField]
    public RectTransform Target;
    void Start()
    {
        ((RectTransform)this.transform).DOAnchorPosY(Target.anchoredPosition.y, Duration).SetRelative(false).From().Play();
    }
}
