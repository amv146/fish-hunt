using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.InputSystem;

public delegate void OnFishTouch(Fish fish);

public class Crosshair : MonoBehaviourPunCallbacks
{
    private SpriteRenderer crosshairRenderer;
    private PhotonView myPhotonView;
    private Animator animator;
    public RuntimeAnimatorController opponentAnimationController;
    public Sprite OpponentCrosshair;
    public OnFishTouch OnFishTouch;
    private Camera mainCamera;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        this.mainCamera = Camera.main;
        this.animator = GetComponent<Animator>();
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        crosshairRenderer = GetComponent<SpriteRenderer>();
        myPhotonView = GetComponent<PhotonView>();
        if (!myPhotonView.IsMine)
        {
            Debug.Log("Crosshair Changed");
            crosshairRenderer.sprite = OpponentCrosshair;
            animator.runtimeAnimatorController = opponentAnimationController;
        }
    }

    private void Update() {
        if (myPhotonView.IsMine)
        {
            ConstrainPosition();
        }
    }

    private void HandleInputs()
    {
        
    }

    public void PlayShootAnimation() {
        animator.Play("P1Crosshair Animation");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<Fish>(out Fish fish))
        {
            if (fish.GetState() == FishState.Alive)
            {
                OnFishTouch?.Invoke(fish);
                return;
            }
        }
        OnFishTouch?.Invoke(null);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnFishTouch?.Invoke(null);
    }

    private void ConstrainPosition()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        if (pos.x > Screen.width)
        {
            pos.x = Screen.width;
        }
        if (pos.x < 0)
        {
            pos.x = 0;
        }
        if (pos.y > Screen.height)
        {
            pos.y = Screen.height;
        }
        if (pos.y < 0)
        {
            pos.y = 0;
        }

        transform.position = this.mainCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10));
    }


}
