using Photon.Pun;
using UnityEngine;
public partial class MainMenu {
    
    public void Play() {
        this.PlayButton.gameObject.SetActive(false);
        this.CancelButton.gameObject.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick Start");
    }

    public void Cancel() {
        CancelButton.gameObject.SetActive(false);
        PlayButton.gameObject.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }


    public void Quit() {
        Application.Quit();
    }

}