using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GrowInOnSpawn : MonoBehaviour
{
    [SerializeField]
    public float minT = 0.5f;
    [SerializeField]
    public float maxT = 0.9f;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(0, Random.Range(minT, maxT)).From().Play();
        transform.localScale = Vector3.zero;
    }
}
