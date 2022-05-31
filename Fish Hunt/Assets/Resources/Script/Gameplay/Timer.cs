using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public delegate void OnTimerEnd();

public class Timer : MonoBehaviourPunCallbacks
{
    private float initialTime = 60f;
    public Text timeText;
    
    private bool gameOver = false;
    private bool isReadyToStart = false;
    public event OnTimerEnd OnTimerEnd;


    // Start is called before the first frame update
    private void Awake() {
        setTimeText();
    }

    void Start() {
        StartCoroutine(RPCStartTimer());
    }

    private IEnumerator RPCStartTimer() {
        if (PhotonNetwork.IsMasterClient) {
            yield return new WaitForSeconds(0.3f);
            photonView.RPC("StartTimer", RpcTarget.MasterClient, null);
        }
    }
    
    [PunRPC]
    void StartTimer() {
        StartCoroutine(RPCUpdateTimer());
    }


    // Update is called once per frame

    private IEnumerator RPCUpdateTimer() {
        yield return new WaitForSeconds(1f);
        photonView.RPC("UpdateTimer", RpcTarget.AllBufferedViaServer, null);
        StartCoroutine(RPCUpdateTimer());
    }

    [PunRPC]
    private void UpdateTimer() {
        initialTime -= 1f;
        setTimeText();
        if (initialTime <= 0)
        {
            gameOver = true;
            if (PhotonNetwork.IsMasterClient) {
                OnTimerEnd?.Invoke();
            }
            StopCoroutine(RPCUpdateTimer());
        }
    }

    void setTimeText()
    {
        timeText.text = "TIME: " + initialTime.ToString("0");
    }
}
