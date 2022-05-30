using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class FishSpawn : MonoBehaviourPunCallbacks
{
    private float time;
    private float spawnTime;
    private float spawnPoint;
    private int goldenChance;
    
    
    public float maxTime = 3;
    public float minTime = 1;
    public float xMin = -10;
    public float xMax = 10;
    public float yFixed = -4;
    

    // Start is called before the first frame update
    void Start()
    {
        SetRandomTime();
        time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if(time >= spawnTime)
        {
            Spawn();
            SetRandomTime();
            time = 0;
        } 
    }

    void Spawn()
    {
        goldenChance = Random.Range(1, 21);
        Vector3 pos = new Vector3(Random.Range(xMin, xMax), yFixed, 0f);
        if (PhotonNetwork.IsMasterClient && goldenChance == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Game", "GoldenFlyingFish"), pos, Quaternion.identity);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Game", "FlyingFish"), pos, Quaternion.identity);
        }
    }

    void SetRandomTime()
    {
        spawnTime = Random.Range(minTime, maxTime);
    }
}
