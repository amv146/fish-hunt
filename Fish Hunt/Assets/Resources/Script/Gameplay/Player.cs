
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
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

public delegate void OnFishShot(Fish fish);

public delegate void OnShot();

public class Player : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public Crosshair crosshair;
    public Shotsboard shotsboard;
    public Scoreboard scoreboard;
    public Score score;
    public Photon.Realtime.Player player;
    private const int MAX_BULLETS = 3;
    private int combo = 0;
    private PlayerMap playerMap;
    private Fish touchingFish = null;
    public OnFishShot OnFishShot;
    public OnShot OnShot;

    private Dictionary<int, int> comboScores = new Dictionary<int, int> {
        {0, 0},
        {1, 0},
        {2, 100},
        {3, 200},
        {4, 300},
        {5, 500}
    };

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        playerMap = new PlayerMap();
        playerMap.Player.Shoot.Enable();
        playerMap.Player.Shoot.performed += HandleShoot;
        object[] data = info.photonView.InstantiationData;

        crosshair = PhotonNetwork.GetPhotonView((int)data[0]).gameObject.GetComponent<Crosshair>();
        scoreboard = PhotonNetwork.GetPhotonView((int)data[1]).gameObject.GetComponent<Scoreboard>();
        shotsboard = PhotonNetwork.GetPhotonView((int)data[2]).gameObject.GetComponent<Shotsboard>();
        score = PhotonNetwork.GetPhotonView((int)data[3]).gameObject.GetComponent<Score>();


        crosshair.OnFishTouch += SetTouchingFish;

        Game.Instance.AddPlayer(this, (string)data[4]);
        ResetBullets();

    }

    public override void OnDisable()
    {
        playerMap.Player.Shoot.performed -= HandleShoot;
        playerMap.Player.Shoot.Disable();
    }

    public void HandleShoot(InputAction.CallbackContext callbackContext)
    {
        if (photonView.IsMine && GetBulletsLeft() > 0)
        {
            OnShot?.Invoke();
            //Handle Animation
            crosshair.GetComponent<Animator>().Play("P1Crosshair Animation");
            if (touchingFish != null)
            {
                AwardPoints(touchingFish.isGolden);
                AddToCombo();
                ResetBullets();
                OnFishShot(touchingFish);
                touchingFish = null;
            }
            else
            {
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

    public Shotsboard GetShotsboard()
    {
        return shotsboard;
    }

    public Crosshair GetCrosshair()
    {
        return crosshair;
    }

    public Scoreboard GetScoreboard()
    {
        return scoreboard;
    }

    public int GetLastScoreAdded()
    {
        if (player.CustomProperties["LastScore"] == null)
        {
            return 0;
        }
        return (int)player.CustomProperties["LastScore"];
    }

    public int GetBulletsLeft()
    {
        if (player.CustomProperties["Bullets"] == null)
        {
            return 0;
        }
        return (int)player.CustomProperties["Bullets"];
    }

    public int GetScore()
    {
        return player.GetScore();
    }

    public int GetComboScore()
    {
        if (combo > 5)
        {
            return comboScores[5];
        }
        return comboScores[combo];
    }

    /**
     * END GETTERS
     */



    /**
     * BEGIN SETTERS
     */

    public void SetTouchingFish(Fish fish)
    {
        touchingFish = fish;
    }

    public void SetBulletsTo(int bullets)
    {
        Hashtable playerHashtable = new Hashtable();
        playerHashtable.Add("Bullets", bullets);
        player.SetCustomProperties(playerHashtable);
    }

    public void SetLastScoreAddedTo(int score)
    {
        Hashtable playerHashtable = new Hashtable();
        playerHashtable.Add("LastScore", score);
        player.SetCustomProperties(playerHashtable);
    }

    public void AddToCombo()
    {
        combo++;
    }

    public void ResetCombo()
    {
        combo = 0;
    }



    public bool IsLocal()
    {
        return PhotonNetwork.LocalPlayer.UserId.Equals(player.UserId);
    }

    public void AddScore(int score)
    {
        player.AddScore(score);
        Debug.Log("score: " + score + " to: " + GetScore());
        SetLastScoreAddedTo(score);
    }

    public void ResetBullets()
    {
        SetBulletsTo(MAX_BULLETS);
        SetLastScoreAddedTo(0);
    }

    public void DecreaseBullets()
    {
        SetBulletsTo(GetBulletsLeft() - 1);
    }



    public void UpdateShotsboard()
    {
        shotsboard.UpdateBullets(GetBulletsLeft());
    }

    public void UpdateScoreboard()
    {
        scoreboard.SetScoreString(GetScore());
    }

    public void UpdateScoreText()
    {
        this.score.SetScoreString(GetLastScoreAdded());
    }

    public override string ToString()
    {
        if (IsLocal())
        {
            return "P1";
        }
        else
        {
            return "P2";
        }
    }

    public void HandleWinOrLoss(bool didWin)
    {
        Hashtable preferences = new Hashtable();
        preferences.Add("DidWin", didWin);
        player.SetCustomProperties(preferences);

        PhotonNetwork.LoadLevel(2);
    }

    public void HandleTie()
    {
        Hashtable preferences = new Hashtable();
        preferences.Add("DidTie", true);
        player.SetCustomProperties(preferences);

        PhotonNetwork.LoadLevel(2);
    }

    IEnumerator RefillBullets()
    {
        yield return new WaitForSeconds(3.5f);

        ResetBullets();
    }

    public void AwardPoints(bool golden)
    {
        int multiplier = 1;
        if (golden)
        {
            multiplier = 10;
        }

        int score = GetComboScore();

        if (GetBulletsLeft() == 3)
        {
            score += (500 * multiplier);
        }
        else if (GetBulletsLeft() == 2)
        {
            score += (400 * multiplier);
        }
        else
        {
            score += (300 * multiplier);
        }

        AddScore(score);
        UpdateScoreText();
    }
}