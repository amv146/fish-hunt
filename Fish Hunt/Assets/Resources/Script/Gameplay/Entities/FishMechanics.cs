using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FishMechanics : MonoBehaviourPunCallbacks
{
    private Rigidbody2D physics;
    private Vector2 movement;
    private int constantSpeed = 1;
    public bool isGolden = false;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody2D>();
        if (isGolden) {
            constantSpeed = 3;
            movement = new Vector2(Random.Range(-300, 300), Random.Range(0, 400));
        }
        movement = new Vector2(Random.Range(-100, 100), Random.Range(0, 200));
        physics.AddForce(movement);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) {
            physics.velocity = Vector2.zero;
            transform.localEulerAngles = new Vector3(0, 0, -90);
            physics.AddForce(new Vector2(0f, -80f));
        }
        else {
            Vector2 newVelocity = SetVelocity(physics.velocity);
            physics.velocity = newVelocity;
            FaceDirection();
        }
    }

    void FaceDirection()
    {
        Vector2 moveDirection = gameObject.GetComponent<Rigidbody2D>().velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private Vector2 SetVelocity(Vector2 fishVelocity)
    {
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
        if (physics == null) {
            return;
        }
        physics.AddForce(new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)));
        physics.velocity = constantSpeed * (physics.velocity.normalized);
    }

    public void FishDied() {
        isDead = true;
        GetComponent<Animator>().enabled = false;
        transform.localEulerAngles = new Vector3(0, 0, -90);
    }
}
