using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class HoundsPlayer : MonoBehaviourPunCallbacks
{
    #region Variables
    GameManager gameManager;
    [Header("Layer Mask")]
    public LayerMask HoundMask;
    public LayerMask TileMask;

    [Space]
    [Header("Player Camera")]
    public Camera playerCamera;

    [Space]
    public GameObject SelectedHound = null;
    #endregion

    #region Unity
    private void Awake()
    {
        gameManager = GameManager.instance;
        photonView.RPC("SetHounder", RpcTarget.All);
    }

    private void Start()
    {
        Debug.Log("he");

    }

    private void Update()
    {
        //Debug.Log(Input.mousePosition);
        if (SelectedHound == null)
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,HoundMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectedHound = hit.collider.gameObject;
                //photonView.RPC("ChangeSelectedHound", RpcTarget.AllBuffered);
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
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectedHound = null;
        }
    }

    public void MoveHound(GameObject _tile)
    {
        if (CheckLegalMove(_tile))
        {
            //photonView.RPC("MovePiece", RpcTarget.AllBuffered, SelectedHound, _tile.transform.GetChild(0).transform.position);
            SelectedHound.transform.position = _tile.transform.GetChild(0).transform.position;
            SelectedHound = null;
            if (gameManager.CheckHoundsWin())
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
        else
        {
            SelectedHound = null;
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

        for (int i = 0; i < gameManager.TilePoints[SelectedHound.GetComponent<Hound>().TileNumber].GetComponent<Tile>().tiles.Count; i++)
        {
            Debug.Log(gameManager.TilePoints[SelectedHound.GetComponent<Hound>().TileNumber].GetComponent<Tile>().tiles[i].name);
            if (_tile.name == gameManager.TilePoints[SelectedHound.GetComponent<Hound>().TileNumber].GetComponent<Tile>().tiles[i].name)
            {
                if (CheckTileIsOccupied(tileNumber))
                {
                    return false;
                }
                int idNumber = SelectedHound.GetComponent<PhotonView>().ViewID;
                photonView.RPC("UpdateTileNumber", RpcTarget.AllBuffered, tileNumber, idNumber);
                //SelectedHound.GetComponent<Hound>().TileNumber = tileNumber;
                return true;
            }
        }
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
    public void SetHounder()
    {
        gameManager.ActiveHounder = this.gameObject;
    }

    [PunRPC]
    public void UpdateTileNumber(int _tileNumber, int _photonID)
    {
       PhotonView.Find(_photonID).gameObject.GetComponent<Hound>().TileNumber = _tileNumber;
       //SelectedHound.GetComponent<Hound>().TileNumber = _tileNumber;
    }
    #endregion
}
