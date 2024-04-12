using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
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
            object[] data = (object[]) photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewID = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUIText = RacingGameManager.instance.finisherTextUI[finishOrder - 1];
            orderUIText.SetActive(true);

            if (viewID == photonView.ViewID)
            {
                orderUIText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer
                    + ("(YOU)");
                orderUIText.GetComponent<Text>().color = Color.red;
            }
            else
            {
                orderUIText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
            }

            
        }


    }

    void Start()
    {
        foreach (GameObject go in RacingGameManager.instance.lapTrigger)
        {
            lapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (lapTriggers.Contains(col.gameObject))
        {
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);
            lapTriggers[indexOfTrigger].SetActive(false);
        }

        if (col.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<CarMovement>().enabled = false;

        finishOrder++;
        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        object[] data = new object[] { nickName, finishOrder, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };
        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOption);
    }
}
