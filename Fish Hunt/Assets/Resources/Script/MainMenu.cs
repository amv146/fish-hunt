using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public static MainMenu Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject PlayButton;
    public GameObject CancelButton;
    public GameObject QuitButton;
    public Text WaitingText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play() {
        MainMenu.Instance.PlayButton.SetActive(false);
        MainMenu.Instance.CancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick Start");
    }

    public void Cancel() {
        PhotonNetwork.LeaveRoom();
    }

    public static void Show(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void Hide(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Quit() {
        Application.Quit();
    }

    public void SetWaitingText() {
        WaitingText.text = "WAITING FOR ANOTHER PLAYER...";
    }
}

