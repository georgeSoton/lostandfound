using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MinesweeperTileBehaviour : MonoBehaviour
{
    
    public bool isBomb;
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
    Transform coverImage;

    MineSweeperManager manager;
    Image baseImage;

    private void Start()
    {
        baseImage = GetComponent<Image>();
        manager = GameObject.FindWithTag("MinigameManager").GetComponent<MineSweeperManager>();
        SetBomb(isBomb);
    }

    public void SetBomb(bool isBomb)
    {
        this.isBomb = isBomb;
        if (isBomb) baseImage.sprite = bombSprite;
        else baseImage.sprite = clearSprite;
    }

    public void TileClicked()
    {
        Tween tw = coverImage.DOScale(0f, tileShrinkTime).Play();
        tw.OnComplete(TileRevealed);
        if (isBomb)
        {
            ps.Play();
        }
    }

    void TileRevealed()
    {
        coverImage.gameObject.SetActive(false);
        if (isBomb)
        {
            explosion.SetActive(true);
            Invoke(nameof(HideBomb), hideBombDelay);
            //TODO: Score Penalty
        }
    }

    void HideBomb()
    {
        baseImage.enabled = false;
        ps.Stop();
    }

}
