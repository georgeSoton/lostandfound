using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Local2DTile : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI tm;
    [SerializeField] Color SelectedBorderColour;
    [SerializeField] UnityEngine.UI.Image Border;
    [SerializeField] BoxCollider2D Collider;
    [SerializeField] UnityEngine.UI.Image InnerArea;
    [SerializeField] float ColourTransitionDuration = 0.5f;
    public event System.Action<string> Selected;
    public string Text
    {
        get => tm.text;
        set => tm.text = value;
    }
    void Start()
    {
    }
    void Update()
    {
        if (InnerArea.rectTransform.hasChanged)
        {
            Collider.size = new Vector2(InnerArea.rectTransform.rect.width, InnerArea.rectTransform.rect.height);
            InnerArea.rectTransform.hasChanged = false;
        }
    }

    public void SelectTile()
    {
        DOTween.Kill(Border);
        Border.DOColor(SelectedBorderColour, ColourTransitionDuration);
    }

    public void DeselectTile()
    {
        DOTween.Kill(Border);
        Border.color = Color.white;
    }

    public void OnMouseDown()
    {
        Debug.Log($"Clicked {Text}");
        if (Selected != null) { Selected.Invoke(Text); }
    }

}
