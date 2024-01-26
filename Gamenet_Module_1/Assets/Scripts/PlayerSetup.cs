using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject m_camera;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] GameObject publicBody;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            m_camera.GetComponent<Camera>().enabled = true;
            publicBody.SetActive(false);
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            m_camera.GetComponent<Camera>().enabled = false;
        }

        playerNameText.text = photonView.Owner.NickName;
    }
}
