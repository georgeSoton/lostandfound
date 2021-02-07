using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithSize : MonoBehaviour
{
    [SerializeField] Vector2 refSize;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector2.one * new Vector2(rectTransform.rect.width, rectTransform.rect.height) / refSize;
    }
}
