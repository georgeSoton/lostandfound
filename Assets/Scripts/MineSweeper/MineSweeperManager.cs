using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MineSweeperManager : MinigameManagerBase
{
    [SerializeField] Transform tileParent;
    [SerializeField] MinesweeperTileBehaviour tilePrefab;
    [SerializeField] int[] tileCountOptions;

    FlexiSquareGridLayout TileGrid;
    List<MinesweeperTileBehaviour> tiles;
    [SyncVar] int tileCount;
    [SyncVar] int bombCount;
    SyncList<bool> bombPositions = new SyncList<bool>();
    SyncList<bool> tilesClearedMap = new SyncList<bool>();
    int correctTilesCleared;

    void Awake()
    {
        minigameType = Minigame.Minesweeper;
        tiles = new List<MinesweeperTileBehaviour>();
        TileGrid = tileParent.GetComponent<FlexiSquareGridLayout>();
        correctTilesCleared = 0;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        tileCount = tileCountOptions[UnityEngine.Random.Range(0, tileCountOptions.Length)];
        for(int i =0; i<tileCount; i++)
        {
            tilesClearedMap.Add(false);
        }
        
        bombCount = UnityEngine.Random.Range(Mathf.Max(1,Mathf.FloorToInt(tileCount * 0.2f)), Mathf.FloorToInt(tileCount*0.8f));
        if (bombCount >= tileCount)
            bombCount = tileCount - 1;
        for (int i =0; i<bombCount; i++)
            bombPositions.Add(true);
        while (bombPositions.Count < tileCount)
            bombPositions.Add(false);

        ShuffleList(bombPositions);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    void Start()
    {
        if (isClient)
        {
            Debug.Log("TileCount: " + tileCount);
            Debug.Log(amPlayer);
            SpawnTiles();
        }
            
    }

    [TargetRpc]
    protected override void TargetMakeAssistant(NetworkConnection conn)
    {
        base.TargetMakeAssistant(conn);
        InitTileState();
        tilesClearedMap.Callback += TileClearedCallback;
    }



    [TargetRpc]
    protected override void TargetMakePlayer(NetworkConnection conn)
    {
        base.TargetMakePlayer(conn);
        InitTileState();
    }

    [Client]
    void InitTileState()
    {
        foreach (var tile in tiles)
        {
            tile.IsBomb = bombPositions[tile.id];
            tile.SetPlayerMode(amPlayer);
            if (amPlayer)
                tile.Selected += TileClicked;
        }
    }

    void SpawnTiles()
    {
        var numOpts = tileCount;
        var sqrt = Mathf.CeilToInt(Mathf.Sqrt((float)numOpts));
        TileGrid.layouts.Clear();
        /*
        for (var i = 1; i < sqrt; i++)
        {
            if (numOpts % i == 0)
            {
                TileGrid.layouts.Add(new Vector2Int(i, numOpts / i));
                TileGrid.layouts.Add(new Vector2Int(numOpts / i, i));
            }
        }*/
        
        // Special case for preceisely square numbers
        if (sqrt * sqrt == numOpts) { TileGrid.layouts.Add(new Vector2Int(sqrt, sqrt)); }
        else { Debug.LogWarning("Only Square Numbers of tiles can be used for Minesweeper"); return; }
        for (int i = 0; i < tileCount; i++)
        {
            //Debug.Log("Setting Tile " + i);
            var parent = TileGrid.transform;
            var tile = Instantiate(tilePrefab, TileGrid.transform);
            tile.id = i;
            tile.tm.text = (i+1).ToString();
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localScale = Vector3.one;
            tiles.Add(tile);
        }
    }


    void TileClicked(int id)
    {
        //End Game Logic
        Debug.Log(nameof(TileClicked) + " called in " + nameof(MineSweeperManager));
        Debug.Log(amPlayer);
        if (amPlayer)
        {
            TileCleared(id);
        }
    }

    [Command(ignoreAuthority = true)]
    void TileCleared(int id)
    {
        Debug.Log(nameof(TileCleared) + " Called");
        if(bombPositions[id]==false)
            correctTilesCleared++;

        tilesClearedMap[id] = true;

        if(correctTilesCleared >= tileCount - bombCount)
        {
            EndMinigame(true);
        }
    }

    private void TileClearedCallback(SyncList<bool>.Operation op, int itemIndex, bool _, bool newItem)
    {
        if (newItem)
        {
            tiles[itemIndex].AssistantReactionTrigger();
        }
    }

    void OnDestroy()
    {
        foreach(var tile in tiles)
        {
            tile.Selected -= TileClicked;
        }    
    }

}
