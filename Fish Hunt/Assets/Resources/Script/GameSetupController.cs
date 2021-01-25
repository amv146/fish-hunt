using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class GameSetupController : MonoBehaviour
{
    public static GameSetupController instance;

    public static GameSetupController Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject ScoreboardPrefab;
    public GameObject ShotsboardPrefab;
    public GameObject ScorePrefab;

    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatePlayer() {
        Debug.Log("Creating Player");
        GameObject crosshair = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "CrosshairPrefab"), Vector3.zero, Quaternion.identity);
        GameObject scoreboard = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "ScoreboardPrefab"), ScoreboardPrefab.transform.position, Quaternion.identity);
        GameObject shotsboard = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "ShotsboardPrefab"), ShotsboardPrefab.transform.position, Quaternion.identity);
        GameObject score = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "ScorePrefab"), ScorePrefab.transform.position, Quaternion.identity);
        Photon.Realtime.Player photonPlayer = PhotonNetwork.LocalPlayer;
        object[] viewIDs = new object[]
        {
            crosshair.GetPhotonView().ViewID, scoreboard.GetPhotonView().ViewID, shotsboard.GetPhotonView().ViewID, score.GetPhotonView().ViewID, photonPlayer.UserId
        };

        GameObject player = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerPrefab"), Vector3.zero, Quaternion.identity, 0, viewIDs);
    }
}
