using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarePlayer : MonoBehaviourPunCallbacks
{
    #region Variables
    GameManager gameManager;
    [Header("Layer Mask")]
    public LayerMask HareMask;
    public LayerMask TileMask;

    [Space]
    [Header("Player Camera")]
    public Camera playerCamera;

    [Space]
    public GameObject SelectedHare = null;
    #endregion

    #region Unity
    private void Awake()
    {
        gameManager = GameManager.instance;
        photonView.RPC("SetHare", RpcTarget.AllBuffered);
        this.enabled = false;
        PhotonView.Find(4).TransferOwnership(this.photonView.Owner);
    }

    private void Start()
    {
        SelectedHare = gameManager.Hare;
        Debug.Log("he");

    }

    private void Update()
    {
        //Debug.Log(Input.mousePosition);
        if (SelectedHare == null)
        {
            SelectHound();
        }
        else
        {
            SelectTile();
        }

    }
    #endregion

    #region Actions
    public void SelectHound()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, HareMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectedHare = hit.collider.gameObject;
            }
        }
    }

    public void SelectTile()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, TileMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (Input.GetKeyDown(KeyCode.Mouse0) && photonView.IsMine)
            {
                MoveHound(hit.collider.gameObject);
            }
        }
    }

    public void MoveHound(GameObject _tile)
    {
        if (CheckLegalMove(_tile))
        {
            //photonView.RPC("MovePiece", RpcTarget.AllBuffered, SelectedHare, _tile.transform.GetChild(0).transform.position);
            SelectedHare.transform.position = _tile.transform.GetChild(0).transform.position;
            if (gameManager.CheckHareWin())
            {
                object[] data = new object[] { photonView.Owner.NickName, "Win", photonView.ViewID };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOption = new SendOptions
                {
                    Reliability = false
                };
                PhotonNetwork.RaiseEvent(1, data, raiseEventOptions, sendOption);
            }
            gameManager.gameObject.GetComponent<PhotonView>().RPC("ChangeTurn", RpcTarget.AllBuffered);
        }
    }

    private bool CheckLegalMove(GameObject _tile)
    {
        int tileNumber = 0;

        for (int i = 0; i < gameManager.TilePoints.Count; i++)
        {
            if (_tile.name == gameManager.TilePoints[i].name)
            {
                tileNumber = i;
                break;
            }
        }

        for (int i = 0; i < gameManager.TilePoints[SelectedHare.GetComponent<Hare>().TileNumber].GetComponent<Tile>().tiles2.Count; i++)
        {
            Debug.Log(gameManager.TilePoints[SelectedHare.GetComponent<Hare>().TileNumber].GetComponent<Tile>().tiles2[i].name);
            if (_tile.name == gameManager.TilePoints[SelectedHare.GetComponent<Hare>().TileNumber].GetComponent<Tile>().tiles2[i].name)
            {
                if (CheckTileIsOccupied(tileNumber))
                {
                    return false;
                }
                photonView.RPC("UpdateTileNumber", RpcTarget.AllBuffered, tileNumber);
                //SelectedHare.GetComponent<Hare>().TileNumber = tileNumber;
                return true;
            }
        }
        Debug.Log(false);
        return false;

    }

    private bool CheckTileIsOccupied(int _tileNumber)
    {
        foreach (GameObject gO in gameManager.Hounds)
        {
            if (gO.GetComponent<Hound>().TileNumber == _tileNumber)
            {
                return true;
            }
        }
        if (gameManager.Hare.GetComponent<Hare>().TileNumber == _tileNumber)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region RPC Functions
    [PunRPC]
    public void SetHare()
    {
        gameManager.ActiveHarer = this.gameObject;
    }

    [PunRPC]
    public void UpdateTileNumber(int _tileNumber)
    {
        SelectedHare.GetComponent<Hare>().TileNumber = _tileNumber;
    }
    #endregion
}
