using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Mirror;
using TMPro;
using System;

public class MinesweeperTileBehaviour : MonoBehaviour
{
    public int id;
    public TextMeshProUGUI tm;
    
    [SerializeField][Range(0f,2f)]
    float tileShrinkTime;
    [SerializeField][Range(0f, 2f)]
    float hideBombDelay;
    [SerializeField]
    Sprite bombSprite;
    [SerializeField]
    Sprite clearSprite;
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    ParticleSystem ps;
    [SerializeField]
    Image coverImage;
    [SerializeField] 
    Color SelectedBorderColour;
    [SerializeField][Range(0f,2f)]
    float ColourTransitionDuration = 0.5f;
    [SerializeField] 
    Image Border;

    bool _isBomb = false;
    public bool IsBomb { get { return _isBomb; } set { SetBomb(value); } }

    Tween twGrowTile; 
    public event Action<int> Selected;
    Image baseImage;

    private void Awake()
    {
        baseImage = GetComponent<Image>();
        twGrowTile = transform.DOScale(0, tileShrinkTime).From().Play();
        //SetBomb(IsBomb);
        coverImage.raycastTarget = false;
    }

    public void SetPlayerMode(bool amPlayer)
    {
        if (amPlayer)
        {
            Debug.Log("Setting Player Mode...");
            coverImage.raycastTarget = true;
        }
        else
        {
            Debug.Log("Setting Assistant Mode...");
            Tween tw = coverImage.transform.DOScale(0f, tileShrinkTime).Pause();
            tw.OnComplete(delegate { coverImage.gameObject.SetActive(false); });
            if (twGrowTile.IsPlaying())
            {
                twGrowTile.OnComplete(delegate { tw.Play(); });
            }
            else
            {
                tw.Play();
            }
            
            if(IsBomb) 
                ps.Play();
        }
    }

    [Client]
    public void SetBomb(bool isBomb)
    {
        this._isBomb = isBomb;
        Debug.Log("bombImage: " + baseImage);
        if (isBomb) baseImage.sprite = bombSprite;
        else baseImage.sprite = clearSprite;
    }

    public void TileClicked()
    {
        Tween tw = coverImage.transform.DOScale(0f, tileShrinkTime).Play();
        tw.OnComplete(TileRevealed);
        if (IsBomb)
        {
            ps.Play();
        }
    }

    void TileRevealed()
    {
        coverImage.gameObject.SetActive(false);
        if (IsBomb)
        {
            explosion.SetActive(true);
            Invoke(nameof(HideBomb), hideBombDelay);
            //TODO: Score Penalty
        }
        else
        {
            HighlightTile();
        }
        if (Selected != null) { Selected.Invoke(id); }
    }

    void HideBomb()
    {
        baseImage.enabled = false;
        ps.Stop();
    }

    public void AssistantReactionTrigger()
    {
        if (IsBomb)
        {
            explosion.SetActive(true);
            Invoke(nameof(HideBomb), hideBombDelay);
        }
        else
        {
            HighlightTile();
        }
    }

    public void HighlightTile()
    {
        Border.DOColor(SelectedBorderColour, ColourTransitionDuration);
    }
}
