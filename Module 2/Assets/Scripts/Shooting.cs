using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;
    public GameManager gameManager;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    public float health;
    public Image healthBar;
    public TextMeshProUGUI nameTag;
    private bool isWin = false;
    private Animator animator;
    private GameObject respawnText;
    public int kills = 20;
    public GameObject killFeedPoint;
    public GameObject killFeedPrefab;
    private bool IsAlive;
    void Start()
    {
        gameManager = GameManager.instance;
        animator = this.GetComponent<Animator>();
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        nameTag.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (kills >= 10)
        {
            isWin = true;
            photonView.RPC("GameOver", RpcTarget.All);
        }
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, 200)) 
        {
            Debug.Log(hit.collider.gameObject.name);
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent
                <PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered,
                    25);

                if (hit.collider.gameObject.GetComponent<Shooting>().health <= 0)
                {
                    kills += 1;
                    Debug.Log(photonView.Owner.NickName + " has " + kills + " kills.");

                }


            }
        }
    }

    [PunRPC]
    public void TakeDamage(int _damage, PhotonMessageInfo info)
    {
        this.health -= _damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0 )
        {
            displayKill(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            //this.gameObject.GetComponent<Collider>().enabled = false;
            Die();
            
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("IsDead", true);
            StartCoroutine(RespawnCountdown());
        }

        
    }

    IEnumerator RespawnCountdown()
    {
        respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;
        Debug.Log("You are killed. Respawning in " + respawnTime.ToString
                (".00"));
        while (respawnTime > 0)
        {
            Debug.Log("inside");
            respawnText.GetComponent<TextMeshProUGUI>().text = "You are killed. Respawning in " + respawnTime.ToString
    (".00");
            yield return new WaitForSeconds(1.0f);
            respawnTime--;
            Debug.Log(respawnTime);
            transform.GetComponent<PlayerMovementController>().enabled = false;

        }

        animator.SetBool("IsDead", false);
        respawnText.GetComponent<TextMeshProUGUI>().text = "";

        int randomPointX = Random.Range(-20, 20);
        int randomPointZ = Random.Range(-20, 20);

        //this.transform.position = new Vector3(randomPointX, 0, randomPointZ);
        
        transform.GetComponent<PlayerMovementController>().enabled = true;
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        
        this.transform.position = GameManager.instance.GetRandomPosition();
        //this.gameObject.GetComponent<Collider>().enabled = true;
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    public void displayKill(string _deathInfo)
    {
        killFeedPoint = GameObject.Find("Point");

        if (killFeedPoint.transform.childCount > 0)
        {
            for (int i = 0; i < killFeedPoint.transform.childCount; i++)
            {
                killFeedPoint.transform.GetChild(i).transform.position += new Vector3(0, -78, 0);
            }
        }

        GameObject newFeed = Instantiate(killFeedPrefab, killFeedPoint.transform);
        Destroy(newFeed, 7f);
        //newFeed.transform.SetParent(killFeedPoint.transform);
        //newFeed.transform.position = Vector3.zero;
        //wFeed.transform.localScale = Vector3.one;
        newFeed.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _deathInfo;
        
    }

    [PunRPC]
    public void GameOver()
    {
        respawnText = GameObject.Find("Respawn Text");
        if (isWin == true)
        {
            respawnText.GetComponent<TextMeshProUGUI>().text = "You Win!";
        }
        else
        {
            respawnText.GetComponent<TextMeshProUGUI>().text = "You Lose.";
        }

        Time.timeScale = 0f;
    }
}
