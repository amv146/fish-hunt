using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Shotsboard : MonoBehaviourPunCallbacks
{
    public GameObject[] Bullets;
    private PhotonView myPhotonView;
    private RectTransform rectTransform;
    private Vector3 p2Position;

    void Update() {
        if (!myPhotonView.IsMine) {
            rectTransform.anchoredPosition = p2Position;
        }
    }

    private void SetToP2() {
        p2Position = new Vector3(690, GameSetupController.Instance.ShotsboardPrefab.transform.position.y);
    }

    private void Start() {
        myPhotonView = GetComponent<PhotonView>();
        rectTransform = GetComponent<RectTransform>();
        if (!myPhotonView.IsMine) {
            SetToP2();
        }
        this.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }


    public void UpdateBullets(int bulletsLeft) {
        if (bulletsLeft < 0) {
            return;
        }
        for (int i = 0; i < bulletsLeft; i++) {
            Bullets[i].SetActive(true);
        }
        for (int i = Bullets.Length - 1; i >= bulletsLeft; i--) {
            Bullets[i].SetActive(false);
        }
    }
}
