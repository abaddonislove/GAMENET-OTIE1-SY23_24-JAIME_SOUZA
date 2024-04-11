using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject FPSModel;
    public GameObject NonFPSModel;

    public GameObject playerUIPrefab;
    public PlayerMovementController playerMovementController;
    public Camera FPSCamera;

    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;

    private Shooting shooting;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        playerMovementController = this.GetComponent<PlayerMovementController>();
        FPSModel.SetActive(photonView.IsMine);
        NonFPSModel.SetActive(!photonView.IsMine);
        animator.SetBool("IsLocalPlayer", photonView.IsMine);

        shooting = this.GetComponent<Shooting>();

        if (photonView.IsMine) 
        {
            this.animator.avatar = fpsAvatar; 

            GameObject playerUI = Instantiate(playerUIPrefab);
            playerMovementController.fixedTouchField = playerUI.transform.Find("RotationTouchField").
                GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent
                <Joystick>();
            FPSCamera.enabled = true;

            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener
                (() => shooting.Fire());
        }
        else
        {
            this.animator.avatar = nonFpsAvatar;
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            FPSCamera.enabled = false;  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
