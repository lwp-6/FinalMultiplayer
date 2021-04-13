using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject LoadPanel;

    private bool connectedToMaster;
    private bool JoinedRoom;
    private LoadingController loadingController;

    private void Awake()
    {
        if (!GlobleVar.isOnline)
        {
            ConnectToMaster();
        }
        loadingController = GetComponentInChildren<LoadingController>();
    }

    private void Update()
    {
        if (GlobleVar.isJoinLobby)
        {
            LoadPanel.SetActive(false);
            return;
        }
        loadingController.RotateLoadIcon(10.0f);
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "Alpha";
    }

    public void CreateRoom(string roomName, byte playerCount, string password, string mapName)
    {
        if(!connectedToMaster || JoinedRoom)
        {
            return;
        }

        ExitGames.Client.Photon.Hashtable tmp_RoomProperties = new ExitGames.Client.Photon.Hashtable();
        tmp_RoomProperties.Add("psw", password);
        tmp_RoomProperties.Add("Map", mapName);
        Debug.Log(tmp_RoomProperties);
        PhotonNetwork.CreateRoom(roomName,
            new RoomOptions
            {
                MaxPlayers = playerCount,
                PublishUserId = true,
                CustomRoomProperties = tmp_RoomProperties,
                CustomRoomPropertiesForLobby = new[] {"psw", "Map"}
            },
            TypedLobby.Default);
    }

    public void JoinRoom()
    {
        if (!connectedToMaster || JoinedRoom)
        {
            return;
        }
        //PhotonNetwork.JoinRoom(RoomName.text);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        connectedToMaster = true;
        GlobleVar.isOnline = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        GlobleVar.isJoinLobby = true;
        Debug.Log("Joined Lobby");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        JoinedRoom = true;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        GlobleVar.isJoinRoom = true;
        //PhotonNetwork.Instantiate(PlayerPrefabName, new Vector3(0, 2, 0), Quaternion.identity);

        //StartSpawn(3);
        //Player.Respawn += StartSpawn;
        PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.CustomProperties["Map"].ToString());
    }

    /*public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }*/

    /*public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Player.Respawn -= StartSpawn;
    }*/

    /*private void StartSpawn(float timeToSpawn)
    {
        StartCoroutine(WaitToInstantiatePlayer(timeToSpawn));
    }

    private IEnumerator WaitToInstantiatePlayer(float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        PhotonNetwork.Instantiate(PlayerPrefabName, new Vector3(0, 2, 0), Quaternion.identity);
    }*/
}
