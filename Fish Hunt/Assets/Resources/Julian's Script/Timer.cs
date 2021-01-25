using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Timer : MonoBehaviourPunCallbacks
{
    private float initialTime = 120f;
    public Text timeText;
    private bool gameOver = false;
    private bool isReadyToStart = false;


    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("StartTimer", RpcTarget.All, null);
    }

    [PunRPC]
    IEnumerator StartTimer() {
        yield return new WaitForSeconds(0.3f);
        isReadyToStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReadyToStart) {
            initialTime -= Time.deltaTime;
            setTimeText();
            if(initialTime <= 0)
            {
                gameOver = true;
            }
        }
    }

    void setTimeText()
    {
        timeText.text = "TIME: " + initialTime.ToString("0");
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
