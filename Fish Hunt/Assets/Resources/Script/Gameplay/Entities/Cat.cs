using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cat : MonoBehaviourPunCallbacks
{
    private GameObject[] deadFish;
    public float speed = 200.0f;
    public float yAxisLimit = -2.0f;
    private PhotonView myPhotonView;
    
    // Start is called before the first frame update
    void Start()
    {
        myPhotonView = this.photonView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        if (GetDeadFish(out deadFish) == true)
        {
            float move = speed * Time.deltaTime;

            if (transform.position.y <= yAxisLimit)
            {
                transform.position = Vector3.MoveTowards(transform.position, deadFish[0].transform.position, move);
            }
            else
            {
                Vector3 newPosition = new Vector3(transform.position.x, yAxisLimit, transform.position.z);
                transform.position = newPosition;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        if (col.gameObject.TryGetComponent<Fish>(out Fish fish)) {
            if (fish.GetState() == FishState.Dead) {
                Fish.OnFishEaten(fish);
            }
        }
    }


    bool GetDeadFish(out GameObject[] deadFish) {
        GameObject[] fish = GameObject.FindGameObjectsWithTag("Fish");
        GameObject[] goldenFish = GameObject.FindGameObjectsWithTag("GoldenFish");

        List<GameObject> tempDeadFish = new List<GameObject>();

        foreach (GameObject tempGoldenFish in goldenFish) {
            if (tempGoldenFish.GetComponent<Fish>().GetState() == FishState.Dead) {
                tempDeadFish.Add(tempGoldenFish);
            }
        }

        foreach (GameObject tempFish in fish) {
            if (tempFish.GetComponent<Fish>().GetState() == FishState.Dead) {
                tempDeadFish.Add(tempFish);
            }
        }

        if (tempDeadFish.ToArray().Length != 0) {
            deadFish = tempDeadFish.ToArray();
            return true;
        }

        deadFish = null;
        return false;
    }
}
