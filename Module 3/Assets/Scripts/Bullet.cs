using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public Camera cam;
    void Update()
    {
        float translation = 100 * Time.deltaTime;
        transform.Translate(Vector3.forward * translation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetPhotonView() != null)
        {
            other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 1f);
        }
        Destroy(this.gameObject);
    }
}
