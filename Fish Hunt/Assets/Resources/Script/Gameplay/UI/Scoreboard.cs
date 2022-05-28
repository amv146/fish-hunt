using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    private RectTransform rectTransform;
    private PhotonView myPhotonView;
    private Vector3 p2Position;
    private bool isMine;

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        myPhotonView = GetComponent<PhotonView>();
        isMine = myPhotonView.IsMine;
        if (!isMine) {
            SetToP2();   
        }
        this.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMine) {
            rectTransform.anchoredPosition = p2Position;
        }
    }

    private void SetToP2() {
        p2Position = new Vector3(675, GameSetupController.Instance.ScoreboardPrefab.transform.position.y);
        SetScoreString(0);
    }

    public void SetScoreString(int score) {
        transform.GetChild(0).GetChild(0).TryGetComponent<Text>(out Text text);
        text.text = GetScoreString(score);
    }

    private string GetScoreString(int score) {
        string scoreString = "";
        if (isMine) {
            scoreString = GetPaddedScore(score) + System.Environment.NewLine + "P1" + " SCORE";
        }
        else {
            scoreString = GetPaddedScore(score) + System.Environment.NewLine + "P2" + " SCORE";
        }
        
        return scoreString;
    }

    private string GetPaddedScore(int score) {
        return ("" + score).PadLeft(8, '0');
    }
}
