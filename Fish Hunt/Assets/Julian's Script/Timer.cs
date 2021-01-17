using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float initialTime = 300f;
    public Text timeText;
    private bool gameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        initialTime -= Time.deltaTime;
        setTimeText();
        if(initialTime <= 0)
        {
            gameOver = true;
        }
    }

    void setTimeText()
    {
        timeText.text = "Time: " + initialTime.ToString("0");
    }

    public bool getGameOver()
    {
        return gameOver;
    }
}
