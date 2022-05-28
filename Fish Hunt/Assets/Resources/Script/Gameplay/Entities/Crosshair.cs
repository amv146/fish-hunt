using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Crosshair : MonoBehaviourPunCallbacks
{
    private SpriteRenderer crosshairRenderer;
    private PhotonView myPhotonView;
    public Sprite OpponentCrosshair;

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        crosshairRenderer = GetComponent<SpriteRenderer>();
        myPhotonView = GetComponent<PhotonView>();
        if (!myPhotonView.IsMine) {
            crosshairRenderer.sprite = OpponentCrosshair;
        }
        InvokeRepeating("HandleInputs", 0, 0.000015f);
    }

    private void HandleInputs() {
        if (myPhotonView.IsMine) {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 10);
            ConstrainPosition();
        }
    }

    private void ConstrainPosition() {
        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        if (Input.mousePosition.x > Screen.width) {
            newPosition.x = Screen.width;
        }
        if (Input.mousePosition.x < 0) {
            newPosition.x = 0;
        }
        if (Input.mousePosition.y > Screen.height) {
            newPosition.y = Screen.height;
        }
        if (Input.mousePosition.y < 0) {
            newPosition.y = 0;
        }

        transform.position = Camera.main.ScreenToWorldPoint(newPosition);
    }
    
    
}
