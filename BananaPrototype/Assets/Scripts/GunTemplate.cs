using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GunTemplate : MonoBehaviour 
{
    
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool shooting, allowButtonHold;
    private int _bulletsLeft, _bulletsShot;
    
    //bools
    private bool readyToShoot, reloading;
    
    //Reference
    public Camera fpsCam;
    public Transform weaponHold;
    public Transform firePoint;
    public LayerMask whatIsEnemy;
    private PlayerControls _playerInput;
    private RaycastHit _rayHit;

    //Graphics
    public GameObject muzzleFlash, bulletImpactEffect;
    public TextMeshProUGUI text;
    
    private void Awake()
    {
        _playerInput = new PlayerControls();

        _playerInput.KeyBoard.Shoot.performed += ctx => MyInput();
        _playerInput.KeyBoard.Shoot.canceled += ctx => MyInput();

        _bulletsLeft = magazineSize;
        readyToShoot = true;

    }
/*
    private void Update()
    {
        text.SetText(_bulletsLeft + " / " + magazineSize);
    }
*/
    private void MyInput()
    {
        if (shooting)
        {
            Debug.Log("CLICKK");
        }
        
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize && !reloading) 
        {
            Reload();
        }
        
        if (readyToShoot && shooting && !reloading && _bulletsLeft > 0)
        {
            _bulletsShot = bulletsPerTap;
            Shoot();
        }
        
    }

    void Shoot()
    {
        readyToShoot = false;
        
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = fpsCam.transform.position + new Vector3(x, y, 0);
        
        //Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out _rayHit, range, whatIsEnemy))
        {
            Debug.Log(_rayHit.collider.name);

            Enemy enemy = new Enemy();

            if (_rayHit.collider.CompareTag("Enemy"))
            {
                _rayHit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        Instantiate(bulletImpactEffect, _rayHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, firePoint.position, Quaternion.identity);
        
        _bulletsLeft--;
        _bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (_bulletsShot > 0 && _bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    void ResetShot()
    {
        readyToShoot = true;
    }
    void Reload()
    {
        reloading = true;
        
        //Call this function when done reloading
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        _bulletsLeft = magazineSize;
        reloading = false;
    }
     private void OnEnable()
    {
        _playerInput.Enable();
    }
       private void OnDisable()
    {
        _playerInput.Disable();
    }
}
