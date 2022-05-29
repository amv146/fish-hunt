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
    }

    private void Update() {
        if (timer.IsGameOver()) {
            HandleWinner();
        }
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
            HandleUpdates(player);
        }
    }

    private void HandleUpdates(Player player) {
        if (player.GetLastScoreAdded() != 0) {
            player.UpdateScoreText();
        }
        player.UpdateShotsboard();
        player.UpdateScoreboard();
    }

    private void OnShot() {
        photonView.RPC("RPC_PlayGunshotSFX", RpcTarget.All, null);
    }

    private void OnFishShot(Fish fish) {
        photonView.RPC("RPC_PlayFishDeathSFX", RpcTarget.All, null);
        fish.Kill();
    }

    

    public void AddPlayer(Player player, string uid) {
        players.Add(player);
        
        foreach (Photon.Realtime.Player photonPlayer in PhotonNetwork.PlayerList) {
            if (photonPlayer.UserId == (string) uid) {
                player.player = photonPlayer;
            }
        }
        if (player.player == null) {
            foreach (Photon.Realtime.Player photonPlayer in PhotonNetwork.PlayerList) {
                if (photonPlayer.UserId == null) {
                    player.player = photonPlayer;
                }
            }
        }
        
        player.OnFishShot += OnFishShot;
        player.OnShot += OnShot;
    }

    public void HandleWinner() {
        PhotonNetwork.AutomaticallySyncScene = false;
        if (players[1].GetScore() > players[0].GetScore()) {
            players[1].HandleWinOrLoss(true);
            players[0].HandleWinOrLoss(false);
            
        }
        else if (players[0].GetScore() > players[1].GetScore()) {
            players[0].HandleWinOrLoss(true);
            players[1].HandleWinOrLoss(false);
        }
        else {
            players[0].HandleTie();
            players[1].HandleTie();
        }
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
