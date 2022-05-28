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

    private void Awake() {
        instance = this;
        
        MainMenu.Instance.PlayButton.gameObject.SetActive(true);
        MainMenu.Instance.CancelButton.gameObject.SetActive(false);

        this.PlayButton.interactable = false;
        print(DISABLED_COLOR);
        this.PlayButtonText.color = DISABLED_COLOR;
        
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayButton.interactable = true;
        print(BUTTON_COLOR);
        this.PlayButtonText.color = BUTTON_COLOR;
    }

    public void SetWaitingText() {
        WaitingText.text = "WAITING FOR ANOTHER PLAYER...";
    }
}

