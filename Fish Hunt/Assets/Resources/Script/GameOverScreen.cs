using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class GameOverScreen : MonoBehaviour
{
    public Text WinText;
    private bool didTie = false;

    // Start is called before the first frame update
    void Start()
    {
        SetWinText();
        PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToMainMenu() {
        PhotonNetwork.LocalPlayer.SetScore(0);
        PhotonNetwork.LoadLevel(0);
    }

    private Photon.Realtime.Player GetNonLocalPlayer() {
        return PhotonNetwork.PlayerListOthers[0];
    }

    private bool DidLocalWin() {
        if (PhotonNetwork.LocalPlayer.GetScore() == GetNonLocalPlayer().GetScore()) {
            didTie = true;
            return false;
        }
        else if (PhotonNetwork.LocalPlayer.GetScore() > GetNonLocalPlayer().GetScore()) {
            return true;
        }
        else {
            return false;
        }
    }

    private void SetWinText() {
        bool didWin = DidLocalWin();

        if (didTie) {
            WinText.text = "You tied with " + PhotonNetwork.LocalPlayer.GetScore() + " points!";
        }
        else if (didWin) {
            WinText.text = "You won with " + PhotonNetwork.LocalPlayer.GetScore() + " points!";
        }
        else {
            WinText.text = "You lost with " + PhotonNetwork.LocalPlayer.GetScore() + " points";
        }
    }
}
