using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MineSweeperManager : MinigameManagerBase
{
    public Transform tileParent;
    public GameObject tilePrefab;

    

    private class Tile
    {
        public GameObject obj;
        public Image coverImg;
        public Image img;
        public Animator anim;
        public bool isBomb;

        public Tile(GameObject obj, bool isBomb, bool amPlayer)
        {
            this.obj = obj;
            coverImg = obj.transform.GetChild(0).GetComponent<Image>();
            anim = anim.GetComponent<Animator>();
            this.isBomb = isBomb;
        }
    }

    FlexiSquareGridLayout TileGrid;
    List<Tile> tiles;
    [SyncVar] int bombCount;
    SyncList<bool> bombPositions = new SyncList<bool>();
    float bombTriggerDelay;

    void Awake()
    {
        tiles = new List<Tile>();
        TileGrid = tileParent.GetComponent<FlexiSquareGridLayout>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        bombCount = Random.Range(2, 9);
        for(int i =0; i<bombCount; i++)
            bombPositions.Add(true);
        while (bombPositions.Count < tileParent.childCount)
            bombPositions.Add(false);
        if (bombCount >= tileParent.childCount)
            bombPositions[0] = false;
        Shuffle(bombPositions);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

    }

    void Start()
    {
        for (int i = 0; i < tileParent.childCount; i++)
        {
            tiles.Add(new Tile(tileParent.GetChild(i).gameObject, bombPositions[i], amPlayer));
        }

        if (amPlayer)
        {

        }
        else
        {

        }
    }

    private void Shuffle<T>(IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }


    [TargetRpc]
    protected virtual void TargetSetOptions(NetworkConnection conn, List<String> options)
    {
        var numOpts = options.Count;
        var sqrt = Mathf.CeilToInt(Mathf.Sqrt((float)numOpts));
        TileGrid.layouts.Clear();
        for (var i = 1; i < sqrt; i++)
        {
            if (numOpts % i == 0)
            {
                TileGrid.layouts.Add(new Vector2Int(i, numOpts / i));
                TileGrid.layouts.Add(new Vector2Int(numOpts / i, i));
            }
        }
        // Special case for preceisely square numbers
        if (sqrt * sqrt == numOpts) { TileGrid.layouts.Add(new Vector2Int(sqrt, sqrt)); }

        foreach (var option in options)
        {
            var parent = TileGrid.transform;
            var tile = Instantiate(TilePrefab);
            tile.Text = option;
            tile.transform.SetParent(parent);
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localScale = Vector3.one;
            myTiles.Add(tile);
            tile.Selected += TileClicked;
        }
    }

}
