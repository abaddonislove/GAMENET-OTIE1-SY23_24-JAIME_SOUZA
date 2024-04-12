using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;

    public int PlayerSelectionNumber;
    // Start is called before the first frame update
    void Start()
    {
        PlayerSelectionNumber = 0;

        ActivatePlayer(PlayerSelectionNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivatePlayer(int x)
    {
        foreach(GameObject go in SelectablePlayers)
        {
            go.SetActive(false);
        }

        SelectablePlayers[x].SetActive(true);

        // setting the player
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable()
            { { Constants.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);

    }

    public void goToNextPlayer()
    {
        PlayerSelectionNumber++;

        if (PlayerSelectionNumber >= SelectablePlayers.Length) 
        {
            PlayerSelectionNumber = 0;
        }

        ActivatePlayer(PlayerSelectionNumber);
    }

    public void goToPreviousPlayer() 
    {
        PlayerSelectionNumber--;
        if (PlayerSelectionNumber < 0)
        {
            PlayerSelectionNumber = SelectablePlayers.Length-1;
        }

        ActivatePlayer(PlayerSelectionNumber);
    }
}
