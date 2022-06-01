using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class FishSpawn : MonoBehaviourPunCallbacks
{
    private float goldenProb = 0.1f;
    private Queue<Fish> freeFish = new Queue<Fish>();
    private string fishPrefabPath = Path.Combine("Prefabs", "Game", "FlyingFish");
    
    public List<Fish> AllFish = new List<Fish>();


    public float maxTime = 3;
    public float minTime = 1;
    public float xMin = -10;
    public float xMax = 10;
    public float yFixed = -4;
    public int maxFish = 7;
    

    // Start is called before the first frame update

    private void Awake() {
        Fish.OnFishEaten += FreeFish;
        if (PhotonNetwork.IsMasterClient) {
            for (int i = 0; i < maxFish; ++i) {
                Fish fish = InstantiateNetworkedFish();
                freeFish.Enqueue(fish);
                AllFish.Add(fish);
            }
        }
    }

    void Start() {
        StartCoroutine(SpawnCycle());
    }


    private IEnumerator SpawnCycle() {
        float randomTime = GetRandomTime();
        RespawnFish();
        yield return new WaitForSeconds(randomTime);
        
        
        
        StartCoroutine(SpawnCycle());
    }

    private Fish InstantiateNetworkedFish() {
        Fish fish = PhotonNetwork.Instantiate(fishPrefabPath, new Vector3(-100f, 0), Quaternion.identity).GetComponent<Fish>();
        fish.SetGolden(ShouldSetGolden());
        return fish;
    }
    
    private void RespawnFish() {
        if (freeFish.Count == 0) {
            return;
        }
        Fish fish = freeFish.Dequeue();
        fish.transform.position = new Vector3(Random.Range(xMin, xMax), yFixed);
        fish.Respawn();
        fish.SetGolden(ShouldSetGolden());
    }
    
    private void FreeFish([NotNull] Fish fish) {
        freeFish.Enqueue(fish);
        fish.RPCSetState(FishState.Queued);
        fish.transform.position = new Vector3(-100, yFixed);
    }
    
    private bool ShouldSetGolden() {
        return Random.Range(0f, 1f) < goldenProb;
    }

    float GetRandomTime()
    {
        return Random.Range(minTime, maxTime);
    }
}
