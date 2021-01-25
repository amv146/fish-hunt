using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex = 1;
    private const float MAX_TIME = 5;
    private float timerToStartGame = MAX_TIME;
    private PhotonView myPhotonView;
    private bool startingGame = false;

    private void Start() {
        myPhotonView = GetComponent<PhotonView>();
    }

    private void Update() {
        WaitingForMorePlayers();
    }

    void WaitingForMorePlayers() {
        if (PhotonNetwork.CurrentRoom == null) {
            timerToStartGame = MAX_TIME;
            return;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) {
            MainMenu.Instance.SetWaitingText();
            timerToStartGame = MAX_TIME;
            return;
        }
        if (CanStartGame()) {
            timerToStartGame -= Time.deltaTime;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        MainMenu.Instance.WaitingText.text = "Player Found! Starting game in " + tempTimer;
        if (timerToStartGame <= 0f) {
            if (startingGame) {
                return;
            }
            StartGame(); 
        }
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
        TryToStartGame();
    }

    private bool CanStartGame() {
        return PhotonNetwork.CurrentRoom.PlayerCount == 2;
    }

    private void TryToStartGame() {
        if (CanStartGame()) {
        }
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn) {
        timerToStartGame = timeIn;
        if (timeIn < MAX_TIME) {

        }
    }

    
    private void StartGame() {
        startingGame = true;
        Debug.Log("Starting Game");
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }
}
