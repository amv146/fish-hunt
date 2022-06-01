
 using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
 using Resources.Script.Gameplay;
 using UnityEngine.InputSystem;
 using Hashtable = ExitGames.Client.Photon.Hashtable;

/**
public enum Player {
    [Description("P1")]
    P1,
    [Description("P2")]
    P2
};
*/

public enum GameResult {
    [Description("Win")]
    Win,
    [Description("Lose")]
    Lose,
    [Description("Tie")]
    Tie
};
 

 public delegate void OnFishShot(Fish fish);

 public delegate void OnShot();
 
public class Player : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback {
    private PlayerModel player;
    public OnFishShot OnFishShot;
    public OnShot OnShot;
    private PlayerMap playerMap;
    private const int MAX_BULLETS = 3;

    
    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        playerMap = new PlayerMap();
        playerMap.Player.Shoot.Enable();
        playerMap.Player.Shoot.performed += HandleShoot;
        object[] data = info.photonView.InstantiationData;

        player = new PlayerModel(data);
        Game.Instance.AddPlayer(this);
        ResetBullets();

    }

    public override void OnDisable() {
        playerMap.Player.Shoot.performed -= HandleShoot;
        playerMap.Player.Shoot.Disable();
    }

    public void HandleShoot(InputAction.CallbackContext callbackContext) {
        if (photonView.IsMine && player.GetBulletsLeft() > 0) {
            OnShot?.Invoke();
            Fish touchingFish = player.GetTouchingFish();
            player.GetCrosshair().PlayShootAnimation();
            if (touchingFish != null) {
                AwardPoints(touchingFish.isGolden);
                AddToCombo();
                ResetBullets();
                OnFishShot.Invoke(touchingFish);
                player.SetTouchingFish(null);
            }
            else {
                DecreaseBullets();
                ResetCombo();
                StopAllCoroutines();
                StartCoroutine(RefillBullets());
            }
        }
    }

    /**
     * BEGIN GETTERS
     */

    public void UpdateViews() {
        if (player.GetLastScoreAdded() != 0) {
            UpdateScoreText();
        }
        UpdateShotsboard();
        UpdateScoreboard();
    }
    

    

    /**
     * END GETTERS
     */



    /**
     * BEGIN SETTERS
     */

    
            
    public void AddToCombo() {
        player.SetCombo(player.GetCombo() + 1);
    }

    public void ResetCombo() {
        player.SetCombo(0);
    }
    
   


    

    public void ResetBullets() {
        player.SetBulletsTo(MAX_BULLETS);
        player.SetLastScoreAddedTo(0);
    }

    public void DecreaseBullets() {
        player.SetBulletsTo(player.GetBulletsLeft() - 1);
    }

    

    public void UpdateShotsboard() {
        player.GetShotsboard().UpdateBullets(player.GetBulletsLeft());
    }

    public void UpdateScoreboard() {
        player.GetScoreboard().SetScoreString(player.GetScore());
    }

    public void UpdateScoreText() {
        this.player.GetScoreText().SetScoreString(player.GetLastScoreAdded());
    }

    public override string ToString() {
        if (player.IsLocal()) {
            return "P1";
        }
        else {
            return "P2";
        }
    }
    
    private void UpdateProperty(string property, object value) {
        Hashtable hash = new Hashtable();
        hash.Add(property, value);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void UpdateGameResult(GameResult result) {
        UpdateProperty("GameResult", result.ToString());
    }

    public void HandleTie() {
        Hashtable preferences = new Hashtable();
        preferences.Add("DidTie", true);
        player.GetPhotonPlayer().SetCustomProperties(preferences);

        PhotonNetwork.LoadLevel(2);
    }
    
    IEnumerator RefillBullets() {
        yield return new WaitForSeconds(3.5f);

        ResetBullets();
    }
    
    public void AwardPoints(bool golden) {
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
    }
    
    public static bool operator<(Player p1, Player p2) {
        return p1.player.GetScore() < p2.player.GetScore();
    }
    
    public static bool operator>(Player p1, Player p2) {
        return p1.player.GetScore() > p2.player.GetScore();
    }
    
    public static bool operator ==(Player p1, Player p2) {
        return p1.player.GetScore() == p2.player.GetScore();
    }
    
    public static bool operator !=(Player p1, Player p2) {
        return p1.player.GetScore() != p2.player.GetScore();
    }
}