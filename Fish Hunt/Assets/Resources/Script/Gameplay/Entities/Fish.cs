using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public enum FishState {
    Alive,
    Dead,
    Queued
}
public delegate void OnFishEaten(Fish fish);

public class Fish : MonoBehaviourPunCallbacks {
    private static readonly float GOLD_SPEED = 1.5f;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 velocity;
    private int constantSpeed = 1;
    public bool isGolden = false;
    private FishState state = FishState.Queued;
    public Animator animator;

    private Vector2 minVelocity = new Vector2(-5, -5);
    private Vector2 maxVelocity = new Vector2(5, 5);
    
    public object lockObject = new object();

    public static RuntimeAnimatorController FishAnimatorController;
    public static RuntimeAnimatorController GoldenFishAnimatorController;
    public static Sprite FishSprite;
    public static Sprite GoldenFishSprite;
    public static OnFishEaten OnFishEaten;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (isGolden) {
            constantSpeed = 2;
        }
    }
    
    void FaceDirection(Vector2 velocity)
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion rot1 = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rot2 = Quaternion.Euler(0, 0, angle);
        transform.eulerAngles = Vector3.forward * angle;
        print(velocity + " " + angle + " " + rot1 + " " + rot2 + " " + transform.eulerAngles);   
    }

    [PunRPC]
    public void SetRandomVelocity(float xDir, float yDir) {
        Vector2 newVelocity = GetRandomVelocity() * (isGolden ? GOLD_SPEED : 1);
        newVelocity.x = (xDir != 0) ? Math.Abs(newVelocity.x) * xDir : newVelocity.x;
        newVelocity.y = (yDir != 0) ? Math.Abs(newVelocity.y) * yDir : newVelocity.y;
        rigidBody.velocity = newVelocity;
        FaceDirection(newVelocity);
    }

    public void SetGolden(bool isGolden) {
        this.isGolden = isGolden;
        if (isGolden) {
            constantSpeed = 2;
            spriteRenderer.sprite = GoldenFishSprite;
            animator.runtimeAnimatorController = GoldenFishAnimatorController;
        } else {
            constantSpeed = 1;
            spriteRenderer.sprite = FishSprite;
            animator.runtimeAnimatorController = FishAnimatorController;
        }
    }

    public void Respawn() {
        RPCSetState(FishState.Alive);
        RPCEnableAnimator();
        SetRandomVelocity(0, 1);
    }



    void OnCollisionEnter2D(Collision2D collision) {
        lock (lockObject) {
            Vector2 closestPoint =
                (collision.collider.bounds.ClosestPoint(collider.bounds.center) - collider.bounds.center).normalized;
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }

            if (state == FishState.Dead) {
                rigidBody.velocity = Vector2.zero;
                return;
            }

            SetRandomVelocity(-closestPoint.x, -closestPoint.y);
        }
    }

    public void RPCSetState(FishState state) {
        photonView.RPC("SetState", RpcTarget.AllViaServer, state);
    }
    
    public void RPCEnableAnimator() {
        photonView.RPC("EnableAnimator", RpcTarget.AllViaServer);
    }
    
    [PunRPC]
    public void EnableAnimator() {
        animator.enabled = true;
    }

    [PunRPC]
    private void SetState(FishState state) {
        this.state = state;
    }
    
    public FishState GetState() {
        return state;
    }
    
    
    
    private Vector2 GetRandomVelocity() {
        float xMult = ((Random.Range(0, 2) == 0) ? 1 : -1);
        float yMult = ((Random.Range(0, 2) == 0) ? 1 : -1);
        return new Vector2( xMult * Random.Range(1f, 5f),  yMult * Random.Range(1f, 5f));
    }

    public void Kill() {
        print("Killed");
        photonView.RPC("HandleDeath", RpcTarget.AllViaServer);
    } 
    
    [PunRPC]
    private void HandleDeath() {
        lock (lockObject) {
            rigidBody.velocity = Vector2.down * 5;
            transform.localEulerAngles = new Vector3(0, 0, -90);
            RPCSetState(FishState.Dead);
            animator.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("DeadFish");
        }
    }
}
