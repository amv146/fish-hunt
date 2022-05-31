using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Script.Gameplay {
    public class PlayerModel {
        private Crosshair crosshair;
        private Shotsboard shotsboard;
        private Scoreboard scoreboard;
        private Score score;
        private Photon.Realtime.Player photonPlayer;
        private int combo = 0;
        private Fish touchingFish = null;
        
        private Dictionary<int, int> comboScores = new Dictionary<int, int> {
            {0, 0},
            {1, 0},
            {2, 100},
            {3, 200},
            {4, 300},
            {5, 500}
        };

        public PlayerModel(object[] data) {
            crosshair = PhotonNetwork.GetPhotonView((int)data[0]).gameObject.GetComponent<Crosshair>();
            scoreboard = PhotonNetwork.GetPhotonView((int)data[1]).gameObject.GetComponent<Scoreboard>();
            shotsboard = PhotonNetwork.GetPhotonView((int)data[2]).gameObject.GetComponent<Shotsboard>();
            score = PhotonNetwork.GetPhotonView((int) data[3]).gameObject.GetComponent<Score>();
            photonPlayer = PhotonNetwork.CurrentRoom.GetPlayer((int) data[4]);

            crosshair.OnFishTouch += SetTouchingFish;

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
        
        public Score GetScoreText() {
            return score;
        }
        
        public Fish GetTouchingFish() {
            return touchingFish;
        }
        
        public Photon.Realtime.Player GetPhotonPlayer() {
            return photonPlayer;
        }
        
        public int GetCombo() {
            return combo;
        }
        
        public int GetLastScoreAdded() {
            if (photonPlayer.CustomProperties["LastScore"] == null) {
                return 0;
            }
            return (int) photonPlayer.CustomProperties["LastScore"];
        }

        public int GetBulletsLeft() {
            if (photonPlayer.CustomProperties["Bullets"] == null) {
                return 0;
            }
            return (int)photonPlayer.CustomProperties["Bullets"];
        }

        public int GetScore() {
            return photonPlayer.GetScore();
        }
    
        public int GetComboScore() {
            if (combo > 5) {
                return comboScores[5];
            }
            return comboScores[combo];
        }
        
        public void SetCombo(int combo) {
            this.combo = combo;
        }
        
        
        public void SetTouchingFish(Fish fish) {
            touchingFish = fish;
        }
        
        public void SetBulletsTo(int bullets) {
            Hashtable playerHashtable = new Hashtable();
            playerHashtable.Add("Bullets", bullets);
            photonPlayer.SetCustomProperties(playerHashtable);
        }

        public void SetLastScoreAddedTo(int score) {
            Hashtable playerHashtable = new Hashtable();
            playerHashtable.Add("LastScore", score);
            photonPlayer.SetCustomProperties(playerHashtable);
        }

        public bool IsLocal() {
            return PhotonNetwork.LocalPlayer.UserId.Equals(photonPlayer.UserId);
        }

        public void AddScore(int score) {
            photonPlayer.AddScore(score);
            Debug.Log("score: " + score + " to: " + GetScore());
            SetLastScoreAddedTo(score);
        }
    }
}