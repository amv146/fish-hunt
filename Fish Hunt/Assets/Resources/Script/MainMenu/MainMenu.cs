using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public partial class MainMenu : MonoBehaviourPunCallbacks
{
    private static MainMenu instance;

    public static MainMenu Instance
    {
        get
        {
            return instance;
        }
    }

    public Button PlayButton;
    public Text PlayButtonText;
    public Button CancelButton;
    public Button QuitButton;
    public Text WaitingText;

    private readonly Color BUTTON_COLOR = new Color(50 / 256.0f, 50 / 256.0f, 50 / 256.0f);
    private readonly Color DISABLED_COLOR = new Color(109 / 256.0f, 107 / 256.0f, 107 / 256.0f);

    public override void OnEnable() {
        base.OnEnable();
        instance = this;
        
        
        this.SetCancelButton(false);

        if (!PhotonNetwork.IsConnectedAndReady) {
            SetPlayButtonInteractable(false);
        }
    }

    public override void OnDisable() {
        print("OnDisable");
        this.SetCancelButton(false);
        
        SetPlayButtonInteractable(true);
        WaitingText.text = "";
    }
    
    public override void OnLeftRoom() {
        WaitingText.text = "";
    }

    public override void OnConnectedToMaster() {
        SetPlayButtonInteractable(true);
    }

    private void SetCancelButton(bool active) {
        this.PlayButton.gameObject.SetActive(!active);
        this.CancelButton.gameObject.SetActive(active);
            
    }

    public void SetPlayButtonInteractable(bool interactable) {
        this.PlayButton.interactable = interactable;
        this.PlayButtonText.color = interactable ? BUTTON_COLOR : DISABLED_COLOR;
    }
    

    public void SetWaitingText() {
        WaitingText.text = "WAITING FOR ANOTHER PLAYER...";
    }
}

