using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : Photon.PunBehaviour {

    #region Variables
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public int countryCount;
    byte MaxPlayersPerRoom;
    int mapIndex;
    string _gameVersion = "1";
    bool isConnecting;
    #endregion

    #region MonoBehaviour Methods
    void Awake()
    {
        // Force LogLevel
        PhotonNetwork.logLevel = Loglevel;
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }
    #endregion

    #region Photon Methods
    public void Connect()
    {
        isConnecting = true;
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            Hashtable expectedCustomRoomProperties = new Hashtable() { { "map", mapIndex } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, MaxPlayersPerRoom);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()  
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            Hashtable expectedCustomRoomProperties = new Hashtable() { { "map", mapIndex } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, MaxPlayersPerRoom);
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        RoomOptions roomOptions = new RoomOptions();
        string[] properties = {"map"};
        roomOptions.CustomRoomPropertiesForLobby = properties;
        roomOptions.CustomRoomProperties = new Hashtable() { { "map", mapIndex } };
        roomOptions.MaxPlayers = MaxPlayersPerRoom;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");
            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("MultiPlayerGame");
        }
    }
    #endregion

    #region Set/Get Methods
    public void setMapLocation(int index)
    {
        if(index == -1)
            index = UnityEngine.Random.Range(0, countryCount);
        mapIndex = index;
    }

    public void setPlayerCount(int count)
    {
        MaxPlayersPerRoom = (byte) count;
    }
    #endregion
}
