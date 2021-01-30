using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SlideInFromBelow : MonoBehaviour
{
    [SerializeField]
    public float Duration = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOLocalMoveY(-500, Duration).From().Play();
    }
}
