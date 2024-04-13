using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Variables
    [Header("Board")]
    public List<GameObject> TilePoints = new List<GameObject>();

    [Space]
    [Header("Player Pieces")]
    public List<GameObject> Hounds = new List<GameObject>();
    public GameObject Hare;

    [Space]
    [Header("Players")]
    public GameObject Hounder;
    public GameObject Harer;
    [Space]
    [Header("ActivePlayers")]
    public GameObject ActiveHounder;
    public GameObject ActiveHarer;

    [Space]
    public Text winStatusText;
    public Text playerTurnText;

    public bool isHareTurn = false; // determins who's turn it is.
    public static GameManager instance = null; // ref in other scripts.
    #endregion

    #region Unity
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        playerTurnText.text = "Cops";
        playerTurnText.color = Color.blue;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Hounder.name, Vector3.zero, Quaternion.identity);
            } else
            {
                PhotonNetwork.Instantiate(Harer.name, Vector3.zero, Quaternion.identity);
            }
            
        }

        // setting startPositions
        GameObject[] startingPoints = { TilePoints[10], TilePoints[9], TilePoints[7] };
        for (int i = 0; i < startingPoints.Length; i++)
        {
            Hounds[i].transform.position = startingPoints[i].transform.GetChild(0).transform.position;
            Hounds[i].GetComponent<Hound>().TileNumber = 10 - (i + Mathf.Clamp(i-1,0,1));
        }

        Hare.transform.position = TilePoints[0].transform.GetChild(0).transform.position;
    }
    #endregion

    #region Actions
    public bool CheckHareWin()
    {
        int largestHoundTileNumber = 0;

        foreach (GameObject dog in Hounds)
        {
            if (dog.GetComponent<Hound>().TileNumber > largestHoundTileNumber)
            {
                largestHoundTileNumber = dog.GetComponent<Hound>().TileNumber;
            }
        }

        if (Hare.GetComponent<Hare>().TileNumber > largestHoundTileNumber)
        {
            return true;
        }

        if (largestHoundTileNumber - Hare.GetComponent<Hare>().TileNumber == 1 && (largestHoundTileNumber == 9|| largestHoundTileNumber == 6 || largestHoundTileNumber == 3))
        {
            return true;
        }

        return false;
    }

    public bool CheckHoundsWin()
    {
        int availableTiles = TilePoints[Hare.GetComponent<Hare>().TileNumber].GetComponent<Tile>().tiles2.Count;
        
        if (availableTiles == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (TilePoints[Hare.GetComponent<Hare>().TileNumber].GetComponent<Tile>().tiles2[i].name == (Hounds[j].GetComponent<Hound>().TileNumber + 1).ToString())
                    {
                        availableTiles--;
                    }
                }
            }
        }

        if (availableTiles <= 0)
        {
            return true;
        }

        return false;
    }
    #endregion


    #region RPC Functions
    [PunRPC]
    public void ChangeTurn()
    {
        
        ActiveHounder.transform.GetComponent<HoundsPlayer>().enabled = isHareTurn;
        isHareTurn = !isHareTurn;
        ActiveHarer.transform.GetComponent<HarePlayer>().enabled = isHareTurn;
        if (!isHareTurn)
        {
            playerTurnText.text = "Cops";
            playerTurnText.color = Color.blue;
        } else
        {
            playerTurnText.text = "Robbert";
            playerTurnText.color = Color.red;
        }
    }
    #endregion
}
