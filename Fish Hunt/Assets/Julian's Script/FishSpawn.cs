﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawn : MonoBehaviour
{
    public GameObject fish;
    public GameObject goldenFish;
    public float maxTime = 3;
    public float minTime = 1;
    public float xMin = -10;
    public float xMax = 10;
    public float yFixed = -4;
    private float time;
    private float spawnTime;
    private float spawnPoint;
    private int goldenChance;

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
        if (goldenChance == 1)
        {
            Instantiate(goldenFish, pos, Quaternion.identity);
        }
        else
        {
            Instantiate(fish, pos, Quaternion.identity);
        }
    }

    void SetRandomTime()
    {
        spawnTime = Random.Range(minTime, maxTime);
    }
}
