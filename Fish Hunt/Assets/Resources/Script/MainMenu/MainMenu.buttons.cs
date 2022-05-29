using Photon.Pun;
using UnityEngine;
public partial class MainMenu {
    
    public void Play() {
        this.SetCancelButton(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick Start");
    }

    public void Cancel() {
        PhotonNetwork.LeaveRoom();
        this.SetCancelButton(false);
        
    }




    public void Quit() {
        Application.Quit();
    }

}