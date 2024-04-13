using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class WinTracker : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 1
    }

    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            string winStatus = (string)data[1];
            int viewID = (int)data[2];

            if (viewID == photonView.ViewID)
            {
                instance.winStatusText.text = "You lose..";
            }
            else
            {
                instance.winStatusText.text = "You Win!";
            }
            Debug.Log((viewID == photonView.ViewID) + " " + photonView.ViewID);
            Debug.Log(nickNameOfFinishedPlayer + " " + winStatus + " " + viewID);
        }

    }
}
