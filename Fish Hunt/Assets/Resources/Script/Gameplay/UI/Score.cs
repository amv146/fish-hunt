using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

public class Score : MonoBehaviourPunCallbacks {
    // Use this for initialization
    private RectTransform rectTransform;
    private PhotonView myPhotonView;
    private Vector3 p2Position;
    private bool isMine;
    private Text text;
    private Color textColor;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        myPhotonView = GetComponent<PhotonView>();
        isMine = myPhotonView.IsMine;
        text = gameObject.transform.GetChild(0).GetComponent<Text>();
        if (!isMine) {
            SetToP2();   
        }
        this.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        textColor = text.color;
        text.color = new Color(textColor.r, textColor.g, textColor.b, 0);
    }

    // Update is called once per frame
    void Update() {
        if (!isMine) {
            rectTransform.anchoredPosition = p2Position;
        }
        Color color = text.color;
        text.color = new Color(color.r, color.g, color.b, color.a - 0.02f);
    }

    private void SetToP2() {
        p2Position = new Vector3(675, GameSetupController.Instance.ScorePrefab.transform.position.y);
        SetScoreString(0);
    }

    public void SetScoreString(int score) {
        text.text = GetScoreString(score);
        Color color = text.color;
        text.color = new Color(color.r, color.g, color.b, 1);
    }

    private string GetScoreString(int score) {
        string scoreString = "+" + score;
        
        return scoreString;
    }
}
