using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fish : MonoBehaviourPunCallbacks {
    private static readonly float GOLD_SPEED = 1.5f;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
    private Vector2 velocity;
    private int constantSpeed = 1;
    public bool isGolden = false;
    public bool isDead = false;

    private Vector2 minVelocity = new Vector2(-5, -5);
    private Vector2 maxVelocity = new Vector2(5, 5);

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        if (isGolden) {
            constantSpeed = 2;
            rigidBody.velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        }
        rigidBody.velocity = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        FaceDirection();
        
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

    private void ChangeVelocity() {
        Vector2 newVelocity = GetRandomVelocity() * (isGolden ? GOLD_SPEED : 1);
        rigidBody.velocity = newVelocity;
        FaceDirection();
    }
    

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        if (isDead) {
            rigidBody.velocity = Vector2.zero;
            return;
        }

        ChangeVelocity();
    }
    
    private Vector2 GetRandomVelocity() {
        return new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
    }

    public void Kill() {
        if (PhotonNetwork.IsMasterClient) {
            HandleDeath();
        }
    } 
    
    [PunRPC]
    private void HandleDeath() {
        isDead = true;
        GetComponent<Animator>().enabled = false;
        rigidBody.velocity = Vector2.down * 5;
        transform.localEulerAngles = new Vector3(0, 0, -90);
        gameObject.layer = LayerMask.NameToLayer("DeadFish");
        collider.sharedMaterial = null;
    }
}
