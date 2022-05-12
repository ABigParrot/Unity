using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public GunTemplate gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;
    private PlayerControls _pCon;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped, weaponGrab;
    public static bool slotFull;

    private void Awake()
    {
        _pCon = new PlayerControls();

        _pCon.KeyBoard.Interact.performed += ctx => MyInput();
        _pCon.KeyBoard.Interact.canceled += ctx => MyInput();
        
        _pCon.KeyBoard.Drop.performed += ctx => MyInput();
        _pCon.KeyBoard.Drop.canceled += ctx => MyInput();
    }

    private void MyInput()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && !slotFull)
        {
            PickUp();
        }
        
        if (equipped && distanceToPlayer.magnitude <= pickUpRange && !slotFull)
        {
            Drop();
        }
    }
    
    private void PickUp()
    
    {
        equipped = true;
        slotFull = true;
        
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        gunScript.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        rb.isKinematic = false;
        coll.isTrigger = false;

        gunScript.enabled = false;
    }
}
