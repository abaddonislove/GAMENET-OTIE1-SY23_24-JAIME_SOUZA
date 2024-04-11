using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text ConnectionStatusText;

    [Header("Login UI Panel")]
    public InputField PlayerNameInput;
    public GameObject LoginUIPanel;

    [Header("Game Options Panel")]
    public GameObject GameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;
    public InputField RoomNameInputField;
    public InputField PlayerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject ShowRoomListPanel;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    public Text RoomInfoText;
    public GameObject PlayerListItemPrefab;
    public GameObject PlayeListViewParent;
    public GameObject StartGameButton;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;
    public GameObject RoomItemPrefab;
    public GameObject RoomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;


    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        roomListGameObjects = new Dictionary<string, GameObject>();
        cachedRoomList = new Dictionary<string, RoomInfo>();
        ActivatePanel(LoginUIPanel);

        PhotonNetwork.AutomaticallySyncScene = true; 
    }

    // Update is called once per frame
    void Update()
    {
        ConnectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (string.IsNullOrEmpty(playerName)) 
        {
            Debug.Log("Player name is invalid!");
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(roomName)) 
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(PlayerCountInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsPanel);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby) 
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(ShowRoomListPanel);
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby) { PhotonNetwork.LeaveLobby(); }

        ActivatePanel(GameOptionsPanel);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonCLicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
    #endregion

    #region PUN Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to Photon Servers.");
        ActivatePanel(GameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(InsideRoomPanel);

        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(PlayerListItemPrefab);
            playerItem.transform.SetParent(PlayeListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber
                == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
        //Debug.Log(RoomInfoText.text);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListGameObjects();
        StartGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        foreach (RoomInfo info in roomList) 
        {
            Debug.Log(info.Name);

            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList) 
            { 
                if (cachedRoomList.ContainsKey(info.Name)) 
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                // update existing rooms info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }

                cachedRoomList.Add(info.Name, info);
            }
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(RoomItemPrefab);
            listItem.transform.SetParent(RoomListParent.transform);
            listItem.transform.localScale = Vector3.one;

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player count: " +
                info.PlayerCount + " / " + info.MaxPlayers;
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(
                () => OnJoinRoomClicked(info.Name));

            roomListGameObjects.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        Debug.Log("left");
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player _player)
    {
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        GameObject playerItem = Instantiate(PlayerListItemPrefab);
        playerItem.transform.SetParent(PlayeListViewParent.transform);
        playerItem.transform.localScale = Vector3.one;

        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = _player.NickName;
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(_player.ActorNumber
            == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(_player.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StartGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    { 
        foreach (var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        ActivatePanel(GameOptionsPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region Private Methods
    private void OnJoinRoomClicked(string roomName)
    {
        if (PhotonNetwork.InLobby) { PhotonNetwork.LeaveLobby(); }

        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach (var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }

    #endregion

    #region Public Methods
    public void ActivatePanel(GameObject _panelToBeActivated)
    {
        LoginUIPanel.SetActive(_panelToBeActivated.Equals(LoginUIPanel));
        GameOptionsPanel.SetActive(_panelToBeActivated.Equals(GameOptionsPanel));
        CreateRoomPanel.SetActive(_panelToBeActivated.Equals(CreateRoomPanel));
        JoinRandomRoomPanel.SetActive(_panelToBeActivated.Equals(JoinRandomRoomPanel));
        ShowRoomListPanel.SetActive(_panelToBeActivated.Equals(ShowRoomListPanel));
        InsideRoomPanel.SetActive(_panelToBeActivated.Equals(InsideRoomPanel));
        RoomListPanel.SetActive(_panelToBeActivated.Equals(RoomListPanel));
    }

    #endregion
}
