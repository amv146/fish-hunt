using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMechanics : MonoBehaviour
{
    private Rigidbody2D physics;
    private Vector2 movement;
    private int constantSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        physics = GetComponent<Rigidbody2D>();
        movement = new Vector2(Random.Range(-500, 500), Random.Range(-500, 500));
        physics.AddForce(movement);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = SetVelocity(physics.velocity);
        physics.velocity = newVelocity;
        FaceDirection();
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
        physics.AddForce(new Vector2(Random.Range(-500f, 500f), Random.Range(-500f, 500f)));
        physics.velocity = constantSpeed * (physics.velocity.normalized);
    }
}
