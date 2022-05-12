using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordToggle : MonoBehaviour
{
    [SerializeField] private Comp_CameraController _cameraController;
    [SerializeField] private Comp_CharacterController _characterController;

    [Header("Hand Swords")]
    [SerializeField] private GameObject _handSwordLeft;
    [SerializeField] private GameObject _handSwordRight;
    
    [Header("Back Swords")]
    [SerializeField] private GameObject _backSwordLeft;
    [SerializeField] private GameObject _backSwordRight;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraController = GetComponent<Comp_CameraController>();
        _characterController = GetComponent<Comp_CharacterController>();
        
        _handSwordLeft.SetActive(false);
        _handSwordRight.SetActive(false);
      
        _backSwordLeft.SetActive(true);
        _backSwordRight.SetActive(true);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        bool _crouching = _characterController.Crouching;
        if (_cameraController.LockedOn)
        {
            _handSwordLeft.SetActive(true);
            _handSwordRight.SetActive(true);
      
            _backSwordLeft.SetActive(false);
            _backSwordRight.SetActive(false);
        }
        else
        {
            _handSwordLeft.SetActive(false);
            _handSwordRight.SetActive(false);
      
            _backSwordLeft.SetActive(true);
            _backSwordRight.SetActive(true);
        }

        if (_cameraController.LockedOn && _crouching)
        {
             _handSwordLeft.SetActive(false);
            _handSwordRight.SetActive(false);
      
            _backSwordLeft.SetActive(true);
            _backSwordRight.SetActive(true);
        }
    }
}
