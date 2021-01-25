 using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/**
public enum Player {
    [Description("P1")]
    P1,
    [Description("P2")]
    P2
};
*/

public class Player : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback{
    public Crosshair crosshair;
    public Shotsboard shotsboard;
    public Scoreboard scoreboard;
    public Score score;
    public Photon.Realtime.Player player;
    private const int MAX_BULLETS = 3;
    private int combo = 0;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {

        object[] data = info.photonView.InstantiationData;

        crosshair = PhotonNetwork.GetPhotonView((int)data[0]).gameObject.GetComponent<Crosshair>();
        scoreboard = PhotonNetwork.GetPhotonView((int)data[1]).gameObject.GetComponent<Scoreboard>();
        shotsboard = PhotonNetwork.GetPhotonView((int)data[2]).gameObject.GetComponent<Shotsboard>();
        score = PhotonNetwork.GetPhotonView((int) data[3]).gameObject.GetComponent<Score>();

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            if (player.UserId == (string) data[4]) {
                this.player = player;
            }
        }
        if (this.player == null) {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
                if (player.UserId == null) {
                    this.player = player;
                }
            }
        }

        ResetBullets();
        Game.Instance.AddPlayer(this);
    }

    public Shotsboard GetShotsboard() {
        return shotsboard;
    }

    public Crosshair GetCrosshair() {
        return crosshair;
    }

    public Scoreboard GetScoreboard() {
        return scoreboard;
    }

    public int GetLastScoreAdded() {
        if (player.CustomProperties["LastScore"] == null) {
            return 0;
        }
        return (int)player.CustomProperties["LastScore"];
    }

    public int GetBulletsLeft() {
        if (player.CustomProperties["Bullets"] == null) {
            return 0;
        }
        return (int)player.CustomProperties["Bullets"];
    }

    public int GetScore() {
        return player.GetScore();
    }

    public void AddToCombo() {
        combo++;
    }

    public void ResetCombo() {
        combo = 0;
    }

    public int GetComboScore() {
        if (combo <= 1) {
            return 0;
        }
        else if (combo == 2) {
            return 100;
        }
        else if (combo == 3) {
            return 200;
        }
        else if (combo == 4) {
            return 300;
        }
        else if (combo >= 5) {
            return 500;
        }
        return 0;
    }

    public bool IsLocal() {
        return PhotonNetwork.LocalPlayer.UserId.Equals(player.UserId);
    }

    public void AddScore(int score) {
        player.AddScore(score);
        Debug.Log("score: " + score + " to: " + GetScore());
        SetLastScoreAddedTo(score);
    }

    public void ResetBullets() {
        SetBulletsTo(MAX_BULLETS);
        SetLastScoreAddedTo(0);
    }

    public void DecreaseBullets() {
        SetBulletsTo(GetBulletsLeft() - 1);
    }

    public void SetBulletsTo(int bullets) {
        Hashtable playerHashtable = new Hashtable();
        playerHashtable.Add("Bullets", bullets);
        player.SetCustomProperties(playerHashtable);
    }

    public void SetLastScoreAddedTo(int score) {
        Hashtable playerHashtable = new Hashtable();
        playerHashtable.Add("LastScore", score);
        player.SetCustomProperties(playerHashtable);
    }

    public void UpdateShotsboard() {
        shotsboard.UpdateBullets(GetBulletsLeft());
    }

    public void UpdateScoreboard() {
        scoreboard.SetScoreString(GetScore());
    }

    public void UpdateScoreText() {
        this.score.SetScoreString(GetLastScoreAdded());
    }

    public override string ToString() {
        if (IsLocal()) {
            return "P1";
        }
        else {
            return "P2";
        }
    }

    public void HandleWinOrLoss(bool didWin) {
        Hashtable preferences = new Hashtable();
        preferences.Add("DidWin", didWin);
        player.SetCustomProperties(preferences);

        PhotonNetwork.LoadLevel(2);
    }

    public void HandleTie() {
        Hashtable preferences = new Hashtable();
        preferences.Add("DidTie", true);
        player.SetCustomProperties(preferences);

        PhotonNetwork.LoadLevel(2);
    }
}