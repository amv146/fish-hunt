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

    private List<Scoreboard> Scoreboards;
    private List<Crosshair> Crosshairs;
    private List<Shotsboard> Shotsboards;
    private PhotonView myPhotonView;
    private List<Player> players;
    public Timer timer;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
        myPhotonView = GetComponent<PhotonView>();
        Crosshairs = new List<Crosshair>();
        Scoreboards = new List<Scoreboard>();
        Shotsboards = new List<Shotsboard>();
        players = new List<Player>();
    }

    private void Update() {
        HandleInputs();
        if (timer.IsGameOver()) {
            HandleWinner();
        }
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void HandleInputs() {
        if (Input.GetMouseButtonDown(0) && GetLocalPlayer().GetBulletsLeft() > 0) {
            if (Game.Instance.CollidedwithGoldenFish(GetLocalPlayer().GetCrosshair().gameObject, out GameObject goldenFish)) {
                if (!goldenFish.GetComponent<FishMechanics>().isDead) {
                    photonView.RPC("RPC_HandleFishDeath", RpcTarget.MasterClient, goldenFish.GetPhotonView().ViewID);
                    AwardPoints(GetLocalPlayer(), true);
                    GetLocalPlayer().AddToCombo();
                    GetLocalPlayer().ResetBullets();
                }
            }
            else if (Game.Instance.CollidedwithFish(GetLocalPlayer().GetCrosshair().gameObject, out GameObject fish)) {
                if (!fish.GetComponent<FishMechanics>().isDead) {
                    photonView.RPC("RPC_HandleFishDeath", RpcTarget.MasterClient, fish.GetPhotonView().ViewID);
                    AwardPoints(GetLocalPlayer(), false);
                    GetLocalPlayer().AddToCombo();
                    GetLocalPlayer().ResetBullets();
                }
            }
            else {
                GetLocalPlayer().DecreaseBullets();
                GetLocalPlayer().ResetCombo();
                StopAllCoroutines();
                StartCoroutine(RefillBullets(GetLocalPlayer()));
            }
        }
    }

    IEnumerator RefillBullets(Player player) {
        yield return new WaitForSeconds(5);

        player.ResetBullets();
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

    public GameObject[] GetFish() {
        return GameObject.FindGameObjectsWithTag("Fish");
    }

    public GameObject[] GetGoldenFish() {
        return GameObject.FindGameObjectsWithTag("GoldenFish");
    }

    public bool CollidedwithFish(GameObject crosshair, out GameObject fish) {
        foreach (GameObject tempFish in GetFish()) {
            if (tempFish.GetComponent<BoxCollider2D>().bounds.Contains(crosshair.transform.position)) {
                fish = tempFish;
                return true;
            }
        }
        fish = null;
        return false;
    }

    public bool CollidedwithGoldenFish(GameObject crosshair, out GameObject goldenFish) {
        foreach (GameObject tempGoldenFish in GetGoldenFish()) {
            if (tempGoldenFish.GetComponent<BoxCollider2D>().bounds.Contains(crosshair.transform.position)) {
                goldenFish = tempGoldenFish;
                return true;
            }
        }
        goldenFish = null;
        return false;
    }

    public void AddPlayer(Player player) {
        players.Add(player);
    }

    public Player GetLocalPlayer() {
        foreach (Player player in players) {
            if (player.IsLocal()) {
                return player;
            }
        }
        return null;
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

    public void AwardPoints(Player player, bool golden) {
        int multiplier = 1;
        if (golden) {
            multiplier = 10;
        }

        int score = player.GetComboScore();

        if (player.GetBulletsLeft() == 3) {
            score += (500 * multiplier);
        }
        else if (player.GetBulletsLeft() == 2) {
            score += (400 * multiplier);
        }
        else {
            score += (300 * multiplier);
        }

        player.AddScore(score);
        player.UpdateScoreText();
    }

    [PunRPC]
    void RPC_DestroyFish(int viewID) {
        PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }

    [PunRPC]
    void RPC_HandleFishDeath(int viewID) {
        PhotonNetwork.GetPhotonView(viewID).GetComponent<FishMechanics>().FishDied();
    }
}
