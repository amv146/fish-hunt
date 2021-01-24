using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMechanics : MonoBehaviour
{
    private GameObject[] deadFish;
    public float speed = 2.0f;
    public float yAxisLimit = -2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("DeadFish") == true)
        {
            float move = speed * Time.deltaTime;
            deadFish = GameObject.FindGameObjectsWithTag("DeadFish");

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "DeadFish")
        {
            Destroy(collision.gameObject);
        }
    }
}
