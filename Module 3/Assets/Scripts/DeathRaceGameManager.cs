using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class DeathRaceGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] vehichlePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextUI;

    public static DeathRaceGameManager instance = null;

    public Text timeText;
    public int carsLeft;


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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehichlePrefabs[(int)playerSelectionNumber].name, instantiatePosition
                    , Quaternion.identity);

            }
        }

        foreach (GameObject go in finisherTextUI)
        {
            go.SetActive(false);
        }

        carsLeft = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
