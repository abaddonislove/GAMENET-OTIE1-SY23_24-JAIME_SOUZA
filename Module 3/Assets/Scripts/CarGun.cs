using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class CarGun : MonoBehaviourPunCallbacks
{
    public GameObject firePoint;
    public GameObject projectilePrefab;
    public float health = 3;
    public Text timerText;

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
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewID = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUIText = DeathRaceGameManager.instance.finisherTextUI[0];
            orderUIText.SetActive(true);

            orderUIText.GetComponent<Text>().text = nickNameOfFinishedPlayer + " died. ";
            orderUIText.GetComponent<Text>().color = Color.red;
                                
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(FireGun());
        timerText = DeathRaceGameManager.instance.timeText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FireGun()
    {
        while (true)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.transform.position, this.gameObject.transform.rotation);
            bullet.GetComponent<Bullet>().cam = this.GetComponent<PlayerSetup>().camera;
            yield return new WaitForSeconds(1.0f);
        }
    }

    [PunRPC]
    public void TakeDamage(float _damage, PhotonMessageInfo info)
    {
        this.health -= _damage;
        Debug.Log(photonView.Owner.NickName + " took " + _damage + " damage. HP: " + this.health );
        if (health <= 0)
        {
            //displayKill(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            this.GetComponent<PlayerSetup>().camera.enabled = false;
            if (photonView.IsMine)
            {
                PhotonView.Find(info.Sender.ActorNumber * PhotonNetwork.MAX_VIEW_IDS + 1).gameObject.GetComponent<PlayerSetup>().camera.enabled = true;
            }
            //this.gameObject.GetComponent<Collider>().enabled = false;
            Died();
            
        }
    }


    public void Die()
    {
        if (photonView.IsMine)
        {
            Debug.Log("dead");
            //animator.SetBool("IsDead", true);
            //StartCoroutine(RespawnCountdown());
        }
    }

    public void Died()
    {
        GetComponent<PlayerSetup>().camera.enabled = false;
        GetComponent<CarMovement>().enabled = false;
        

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
        DeathRaceGameManager.instance.carsLeft--;
        Destroy(this.gameObject);
    }
}
