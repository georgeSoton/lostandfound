using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RatingBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(1, 3)]
    private int rating=1;
    [SerializeField]
    GameObject[] starInners;
    [SerializeField]
    [Range(0, 3)]
    float showDelay;

    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject star in starInners)
        {
            star.transform.localScale = Vector3.zero;
        }
    }

    public void ShowStars(int rating)
    {
        this.rating = rating;
        if(rating > 0)
        {
            ShowStarRecursive();
        }
    }

    void ShowStarRecursive()
    {
        starInners[index].transform.DOScale(1, 2f).SetEase(Ease.OutElastic).Play();
        index++;
        if (index < rating)
        {
            Invoke(nameof(ShowStarRecursive), showDelay);
        }
    }

}
