using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fish : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rigidBody;
    private Vector2 movement;
    private int constantSpeed = 1;
    public bool isGolden = false;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (isGolden) {
            constantSpeed = 3;
            movement = new Vector2(Random.Range(-300, 300), Random.Range(0, 400));
        }
        movement = new Vector2(Random.Range(-100, 100), Random.Range(0, 200));
        rigidBody.AddForce(movement);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead) {
            Vector2 newVelocity = SetVelocity(rigidBody.velocity);
            rigidBody.velocity = newVelocity;
            FaceDirection();
        }
    }

    void FaceDirection()
    {
        Vector2 moveDirection = rigidBody.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private Vector2 SetVelocity(Vector2 fishVelocity)
    {
        print("Velocity set");
        Vector2 adjustedVelocity = fishVelocity;
        ConstrainValue(adjustedVelocity.x);
        ConstrainValue(adjustedVelocity.y);
        return adjustedVelocity;
    }

    private float ConstrainValue(float value)
    {
        if (value >= 0)
        {
            value = 6;
        }
        else if (value < 0)
        {
            value = -6;
        }
        return value;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) {
            return;
        }
        if (rigidBody == null) {
            return;
        }
        rigidBody.AddForce(new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)));
        rigidBody.velocity = constantSpeed * (rigidBody.velocity.normalized);
    }

    public void Kill() {
        photonView.RPC("HandleDeath", RpcTarget.AllBuffered);
    } 
    
    [PunRPC]
    private void HandleDeath() {
        isDead = true;
        GetComponent<Animator>().enabled = false;
        rigidBody.velocity = Vector2.down * 5;
        transform.localEulerAngles = new Vector3(0, 0, -90);
        GetComponent<BoxCollider2D>().sharedMaterial = null;
    }
}
