using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    private const int roomSize = 2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.AutomaticallySyncScene = true;
        MainMenu.Instance.PlayButton.SetActive(true);
        MainMenu.Instance.CancelButton.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    public void CreateRoom() {
        Debug.Log("Creating Room Now.");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }

    public void QuickCancel() {
        MainMenu.Instance.CancelButton.SetActive(false);
        MainMenu.Instance.PlayButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
