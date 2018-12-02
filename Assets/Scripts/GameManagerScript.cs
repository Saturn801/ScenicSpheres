using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : Photon.PunBehaviour {

    #region Variables
    public GameObject playerPrefab;
    public Text waitingText, playersText, remainingText, countdownText;
    public PanelScript readyButton, lobbyMenu, canvas, countdownMenu;
    public MapScript mapScript;
    public TimerScript timerScript;
    static public GameManagerScript Instance;
    int maxPlayers, time = 3;
    bool lobbyStart = false, roomClosed = false, allReady = false, gameStart = false, countdownStart = false;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        maxPlayers = PhotonNetwork.room.MaxPlayers;
        var roomProperties = PhotonNetwork.room.CustomProperties;
        mapScript.startGame(int.Parse(roomProperties["map"].ToString()));
        Instance = this;
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!lobbyStart && mapScript.getMapLoadedStatus())
            startLobby();
        if (!allReady && lobbyStart)
            updateLobby();
        else if(allReady && !countdownStart)
            startCountdown();     
        else if(countdownStart && time == 0 && !gameStart)
            startGame();   
    }
    #endregion

    #region Script Specific Methods
    private void startLobby()
    {
        lobbyMenu.turnOn();
        canvas.turnOff();
        lobbyStart = true;
    }

    private void updateLobby()
    {
        var players = PhotonNetwork.playerList;
        if (players.Length == maxPlayers)
        {
            roomClosed = true;
            PhotonNetwork.room.IsOpen = false;
        }
        InvokeRepeating("updateLobbyScreen", 0.0f, 0.1f);
        InvokeRepeating("checkPlayersReady", 0.0f, 0.1f);
    }

    private void startCountdown()
    {
        CancelInvoke();
        lobbyMenu.turnOff();
        countdownMenu.turnOn();
        countdownText.text = "Starting Game in:\n" + time;
        InvokeRepeating("reduceCountDown", 1.0f, 1.0f);
        countdownStart = true;
    }

    private void startGame() {
        CancelInvoke();
        countdownMenu.turnOff();
        mapScript.setLocation();
        canvas.turnOn();
        timerScript.setTime(0);
        gameStart = true;
    }

    private void reduceCountDown()
    {    
        time--;
        countdownText.text = "Starting Game in:\n" + time;
    }

    private void checkPlayersReady()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        int readyCount = 0;
        foreach (var player in players)
        {
            var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
            if (playerController.isReady)
                readyCount++;
        }
        if (readyCount == players.Length)
            allReady = true;
    }

    private void updateLobbyScreen()
    {
        // Waiting Text
        if (roomClosed)
            waitingText.text = "Waiting to play...";
        // Players Text
        string playerText = "";
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerView = (PhotonView)player.GetComponent(typeof(PhotonView));
            playerText += playerView.owner.NickName;
            var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
            if (playerController.isReady)
                playerText += " - READY";
            playerText += "\n";
        }
        playersText.text = playerText;
        // Remaining Text
        if (!roomClosed)
        {
            remainingText.text = players.Length + "/" + maxPlayers + " connected...";
        }
        else
        {
            remainingText.text = "Waiting for players to confirm...";
        }
        // Button
        if (roomClosed)
            readyButton.turnOn();
    }

    public void readyButtonClick()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerView = (PhotonView)player.GetComponent(typeof(PhotonView));
            if (playerView.isMine)
            {
                var playerController = (PlayerManager)player.GetComponent(typeof(PlayerManager));
                playerController.setPlayerReady();
            }
        }
    }
    #endregion

    #region Photon Methods
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(2);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
        }
    }
    #endregion
}
