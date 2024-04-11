using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public GameObject[] spawnPoints;

    private void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomPointX = Random.Range(-10, 10);
            int randomPointZ = Random.Range(-10, 10);
/*            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX, 0, randomPointZ),
                Quaternion.identity);*/
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, 6)].transform.position,
                Quaternion.identity);
        }
    }

    #region UICallbacks
    public Vector3 GetRandomPosition()
    {
        Debug.Log(spawnPoints[Random.Range(0, 6)].transform.position);
        return spawnPoints[Random.Range(0, 6)].transform.position;
    }

    #endregion
}
