using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;


public class Game : MonoBehaviourPunCallbacks
{
    public static Game instance;
    public static Game Instance
    {
        get
        {
            return instance;
        }
    }
    public Timer timer;
    public AudioClip GunshotSFX;
    public AudioClip FishDeathSFX;

    private PhotonView myPhotonView;
    private List<Player> players;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Awake() {
        PhotonNetwork.AddCallbackTarget(this);

        audioSource = GameObject.FindGameObjectWithTag("Sounds").GetComponent<AudioSource>();
        instance = this;
        myPhotonView = GetComponent<PhotonView>();
        players = new List<Player>();
        timer.OnTimerEnd += GameOver;
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LoadLevel(0);
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        foreach (Player player in players) {
            player.UpdateViews();
        }
    }

    void GameOver() {
        photonView.RPC("HandleWinner", RpcTarget.MasterClient);
    }

    private void OnShot() {
        photonView.RPC("RPC_PlayGunshotSFX", RpcTarget.All, null);
    }

    private void OnFishShot(Fish fish) {
        photonView.RPC("RPC_PlayFishDeathSFX", RpcTarget.All, null);
        fish.Kill();
    }

    

    public void AddPlayer(Player player) {
        players.Add(player);
        
        player.OnFishShot += OnFishShot;
        player.OnShot += OnShot;
    }

    [PunRPC]
    public void HandleWinner() {
        if (players[1] > players[0]) {
            players[1].UpdateGameResult(GameResult.Win);
            players[0].UpdateGameResult(GameResult.Lose);
        }
        else if (players[0] > players[1]) {
            players[0].UpdateGameResult(GameResult.Win);
            players[1].UpdateGameResult(GameResult.Lose);
        }
        else {
            players[0].UpdateGameResult(GameResult.Tie);
            players[1].UpdateGameResult(GameResult.Tie);
        }
        PhotonNetwork.LoadLevel(2);

    }
    

    

    [PunRPC]
    void RPC_PlayGunshotSFX() {
        audioSource.PlayOneShot(GunshotSFX);
    }

    [PunRPC]
    void RPC_PlayFishDeathSFX() {
        audioSource.PlayOneShot(FishDeathSFX);
    }

    [PunRPC]
    void RPC_HandleFishDeath(int viewID) {
        PhotonNetwork.GetPhotonView(viewID).GetComponent<Fish>().Kill();
    }
}
