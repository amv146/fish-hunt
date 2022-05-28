using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    private const int roomSize = 2;
    
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

}
