using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera cam;

    private void Start()
    {
        if (photonView.IsMine)
        {
            cam.enabled = true;
        }
        else
        {
            cam.enabled = false;
        }
    }
}
