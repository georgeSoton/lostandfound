using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemController : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas _canvas;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    public List<Sprite> sprites;
    public int diceValue;
    public string position= "center";
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform.Rotate(new Vector3(0,0,Random.Range(0f,360f)));
        position = "center";
    }

    public void SetValue(int i)
    {
        diceValue = i;
        GetComponent<Image>().sprite = sprites[diceValue-1];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnBeginDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = .75f;
        _rectTransform.localScale = new Vector3(.8f,.8f,1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _rectTransform.localScale = new Vector3(1f,1f,1f);
        if (_rectTransform.anchoredPosition.y>=300)
        {
            position = "top";
        }else if (_rectTransform.anchoredPosition.y<=-300)
        {
            position = "bot";
        }
        else
        {
            position = "center";
        }
        Debug.Log(diceValue);
    }


    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}
