using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CountDownManager : MonoBehaviourPunCallbacks
{
    public Text timerText;

    public float timeToStartRace = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            timerText = RacingGameManager.instance.timeText;
        } catch
        {
            timerText = DeathRaceGameManager.instance.timeText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                //SetTime(timeToStartRace);
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<CarMovement>().isControlEnabled = true;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            try
            {
                GetComponent<CarGun>().enabled = true;
            }
            catch { }

            try
            {
                GetComponent<LazerShoot>().enabled = true;
            }
            catch { }
        }


        this.enabled = false;
    }

}
