using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using DG.Tweening;
public class MazeManager : NetworkBehaviour
{
    [SerializeField] public Camera PlayerCam;
    [SerializeField] public Camera AssistantCam;

    [SerializeField]
    public GameObject OpenTile;
    [SerializeField]
    public GameObject ClosedTile;
    [SerializeField]
    public Canvas MazeCanvas;
    [SerializeField]
    public RectTransform PlayerLocation;
    //Bottom left corner start
    int[] playerposition = { 0, Mazes.ysize - 1};

    int ChosenMaze;
    NetworkConnection playerID;

    void Start()
    {
        if (isServer)
        {
            ChosenMaze = Random.Range(0, Mazes.mazes.GetLength(0));
            var clientkeys = NetworkServer.connections.Keys.ToArray();
            playerID = NetworkServer.connections[clientkeys[Random.Range(0, clientkeys.Length)]];
            PlayerLocation.anchoredPosition = Mazes.TilePosition(0, Mazes.ysize - 1);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ClientReady();
    }

    [Command(ignoreAuthority=true)]
    void ClientReady(NetworkConnectionToClient conn = null)
    {
        if (conn.connectionId == playerID.connectionId)
        {
            TargetMakePlayer(conn);
        }
        else
        {
            TargetMakeAssistant(conn, ChosenMaze);
        }
    }
    void AddMazeObjects(int maze)
    {
        for (var x = 0; x < Mazes.xsize; x++)
        {
            for (var y = 0; y < Mazes.ysize; y++)
            {
                AddTile(Mazes.mazes[maze, y, x] ? OpenTile : ClosedTile, x, y);
            }
        }
    }
    void AddTile(GameObject tile, int x, int y)
    {
        var instance = Instantiate(tile);
        instance.transform.SetParent(MazeCanvas.transform, false);
        var rt = (RectTransform)instance.transform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Mazes.TilePosition(x, y);
    }

    [TargetRpc]
    void TargetMakePlayer(NetworkConnection conn)
    {
        Debug.Log("Player");
        PlayerCam.gameObject.SetActive(true);
        PlayerCam.enabled = true;
        AssistantCam.gameObject.SetActive(false);
        AssistantCam.enabled = false;
    }
    [TargetRpc]
    void TargetMakeAssistant(NetworkConnection conn, int maze)
    {
        Debug.Log("Assistant");
        PlayerCam.gameObject.SetActive(true);
        PlayerCam.enabled = true;
        AssistantCam.gameObject.SetActive(true);
        AssistantCam.enabled = true;
        AddMazeObjects(maze);
    }

    enum MoveDirection
    {
        U, D, L, R
    }

    bool MovingATM = false;
    [Command(ignoreAuthority=true)]
    void TryMove(MoveDirection dir, NetworkConnectionToClient conn = null)
    {
        if (MovingATM) {return;}
        if (conn.connectionId != playerID.connectionId) {return;}
        int xdif = 0;
        int ydif = 0;
        switch (dir)
        {
            case MoveDirection.U:
                xdif = 0;
                ydif = -1;
                break;
            case MoveDirection.D:
                xdif = 0;
                ydif = 1;
                break;
            case MoveDirection.R:
                xdif = 1;
                ydif = 0;
                break;
            case MoveDirection.L:
                xdif = -1;
                ydif = 0;
                break;
            default:
                return;
        }
        int newx = playerposition[0] + xdif;
        int newy = playerposition[1] + ydif;
        if ((newx < 0) || (newx >= Mazes.xsize) || (newy < 0) || (newy >= Mazes.ysize) ) {return;}
        if (Mazes.mazes[ChosenMaze, newy, newx]) {
            var newloc = Mazes.TilePosition(newx, newy);
            playerposition = new int[] {newx, newy};
            MovingATM=true;
            PlayerLocation.DOAnchorPos(newloc, 0.4f).OnComplete(()=>MovingATM=false).Play();
        }
    }

    bool AxisReadyForNewMovement = true;
    void Update()
    {
        if (isClient)
        {
            if (AxisReadyForNewMovement)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    AxisReadyForNewMovement = false;
                    TryMove(MoveDirection.U);
                    return;
                }
                if (Input.GetAxis("Vertical") < 0)
                {
                    AxisReadyForNewMovement = false;
                    TryMove(MoveDirection.D);
                    return;
                }
                if (Input.GetAxis("Horizontal") > 0)
                {
                    AxisReadyForNewMovement = false;
                    TryMove(MoveDirection.R);
                    return;
                }
                if (Input.GetAxis("Horizontal") < 0)
                {
                    AxisReadyForNewMovement = false;
                    TryMove(MoveDirection.L);
                    return;
                }
            } else
            {
                if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0) {AxisReadyForNewMovement = true;}
            }
        }
    }

}
