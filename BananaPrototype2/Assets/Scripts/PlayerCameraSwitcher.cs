using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerCameraSwitcher : MonoBehaviour
{
   [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
   [SerializeField] private float normalSensitivity;
   [SerializeField] private float aimSensitivity;
   [SerializeField] private LayerMask aimColliderLayerMask;
   [SerializeField] private Transform debugTransform;
   
   private ThirdPersonController _thirdPersonController;
   private StarterAssetsInputs _starterAssetsInputs;

   private void Awake()
   {
      _thirdPersonController = GetComponent<ThirdPersonController>();
      _starterAssetsInputs = GetComponent<StarterAssetsInputs>();

   }

   private void Update()
   {
      Vector3 mouseWorldPosition = Vector3.zero;
      
      Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
      Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
      if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
      {
         debugTransform.position = raycastHit.point;
         mouseWorldPosition = raycastHit.point;
      }

      _thirdPersonController.SetRotateOnMove(false);
      
      Vector3 worldAimTarget = mouseWorldPosition;
      worldAimTarget.y = transform.position.y;
      Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

      transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
      if (_starterAssetsInputs.aim)
      {
         aimVirtualCamera.gameObject.SetActive(true);
         _thirdPersonController.SetSensitivity(aimSensitivity);
      }
      else
      {
         aimVirtualCamera.gameObject.SetActive(false);
         _thirdPersonController.SetSensitivity(normalSensitivity);
      }
     

 
   }

  
}



