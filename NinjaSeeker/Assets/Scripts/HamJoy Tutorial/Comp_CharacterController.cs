using System;
using UnityEngine;

public enum CharacterStance {Standing, Crouching, Proning}
public class Comp_CharacterController : MonoBehaviour
{
    [Header("Speed (Normal, Sprinting)")] 
    [SerializeField] private Vector2 _standingSpeed = new Vector2(0, 0);
    [SerializeField] private Vector2 _crouchingSpeed = new Vector2(0, 0);
    [SerializeField] private Vector2 _proningSpeed = new Vector2(0, 0);
    
    [Header("Capsule (Radius, Height, YOffset)")]
    [SerializeField] private Vector3 _standingCapsule = Vector3.zero;
    [SerializeField] private Vector3 _crouchingCapsule = Vector3.zero;
    [SerializeField] private Vector3 _proningCapsule = Vector3.zero;

    [Header("Sharpness")] 
    [SerializeField] private float _standingRotationSharpness = 10f;
    [SerializeField] private float _crouchingRotationSharpness = 10f;
    [SerializeField] private float _proningRotationSharpness = 10f;
    [SerializeField] private float _moveSharpness = 10f;

    private Animator _animator;
    private CapsuleCollider _collider;
    private Comp_PlayerInput _inputs;
    private Comp_SMBEventCurrator _eventCurrator;
    private Comp_CameraController _cameraController;
    private SwordToggle _swordToggle;

    private float _sprintSpeed;
    private float _runSpeed;
    private float _rotationSharpness;
    private LayerMask _layerMask;
    private CharacterStance _stance;
    private Collider[] _obstructions = new Collider[8];

    private float _targetSpeed;
    private Quaternion _targetRotation;

    private float _newSpeed;
    private Vector3 _newVelocity;
    private Quaternion _newRotation;

    private bool _inAnimation;
    private Vector3 _animatorVelocity;
    private Quaternion _animatorDeltaRotation;
    private string _animAttack = "Base Layer.Attack1";
    
    private const string _standToCrouch = "Base Layer.Base Crouching";
    private const string _standToProne = "Base Layer.Stand To Prone";
    private const string _crouchToStand = "Base Layer.Base Standing";
    private const string _crouchToProne = "Base Layer.Crouch To Prone";
    private const string _proneToStand = "Base Layer.Prone To Stand";
    private const string _proneToCrouch = "Base Layer.Prone To Crouch";

    private bool _strafing;
    private bool _sprinting;
    private bool _crouching;
    private bool _proning;
    private float _strafeParameter;
    private Vector3 _strafeParameterXZ;
    private static readonly int StrafeZ = Animator.StringToHash("StrafeZ");

    public bool Crouching {get => _crouching;}
    public bool Proning {get => _proning;}

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _inputs = GetComponent<Comp_PlayerInput>();
        _eventCurrator = GetComponent<Comp_SMBEventCurrator>();
        _cameraController = GetComponent<Comp_CameraController>();
        _collider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;

        _runSpeed = _standingSpeed.x;
        _sprintSpeed = _standingSpeed.y;
        _rotationSharpness = _standingRotationSharpness;
        _stance = CharacterStance.Standing;
        SetCapsuleDimensions(_standingCapsule);
        
        int _mask = 0;
        for (int i = 0; i < 32; i++)
            if (Physics.GetIgnoreLayerCollision(gameObject.layer, i))
                _mask |= 1 << i;
        _swordToggle = GetComponent<SwordToggle>();
        _layerMask = _mask;
        _eventCurrator.Event.AddListener(OnSMBEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if(_proning)
            return;
        //Movement of the transform
        Vector3 _moveInputVector = new Vector3(_inputs.MoveAxisRightRaw, 0, _inputs.MoveAxisForwardRaw);
        //Where the camera is going
        Vector3 _cameraPlanarDirection = _cameraController.CameraPlanarDirection;
        //Where the camera is facing
        Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);
    
        Vector3 _moveInputVectorOriented = _cameraPlanarRotation * _moveInputVector;

        _strafing = _cameraController.LockedOn;
        if (_strafing)      { _sprinting = _inputs.Sprint.PressedDown() && (_moveInputVector != Vector3.zero);}
        else                { _sprinting = _inputs.Sprint.Pressed() && (_moveInputVector != Vector3.zero);}
        if(_sprinting)      {_cameraController.ToggleLockOn(false);}
    
        if           (_sprinting) { _targetSpeed = _moveInputVector != Vector3.zero ? _sprintSpeed : 0;} 
        else if       (_strafing) { _targetSpeed = _moveInputVector != Vector3.zero ? _runSpeed : 0;}
        else                      { _targetSpeed = _moveInputVector != Vector3.zero ? _runSpeed : 0;}
        _newSpeed = Mathf.Lerp(_newSpeed, _targetSpeed, Time.deltaTime * _moveSharpness);

        //Velocity
        if (_inAnimation){ _newVelocity = _animatorVelocity;}
        else { _newVelocity = _moveInputVectorOriented * _newSpeed; }
        transform.Translate(_newVelocity * Time.deltaTime, Space.World);
        

        //Rotation
        if(_inAnimation){transform.rotation *= _animatorDeltaRotation;}
        
        else if (_strafing)
        {
            Vector3 _toTarget = _cameraController.Target.TargetTransform.position - transform.position;
            Vector3 _planarToTarget = Vector3.ProjectOnPlane(_toTarget, Vector3.up); 
        
            _targetRotation = Quaternion.LookRotation(_planarToTarget);
            _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
            transform.rotation = _newRotation;
        }
        else if (_targetSpeed != 0)
        {
            _targetRotation = Quaternion.LookRotation(_moveInputVectorOriented);
            _newRotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
            transform.rotation = _newRotation;
        }
    
        //Animations
        if (_strafing)
        {
            _strafeParameter = Mathf.Clamp01(_strafeParameter + Time.deltaTime * 4);
            _strafeParameterXZ = Vector3.Lerp(
                _strafeParameterXZ, 
                _moveInputVector * _newSpeed,
                _moveSharpness * Time.deltaTime);
            
        }
        else
        {
            _strafeParameter = Mathf.Clamp01(_strafeParameter - Time.deltaTime * 4);
            _strafeParameterXZ = Vector3.Lerp(
                _strafeParameterXZ, 
                Vector3.forward * _newSpeed,
                _moveSharpness * Time.deltaTime);  
        }
        _animator.SetFloat("Strafing", _strafeParameter);
        _animator.SetFloat("StrafeX", Mathf.Round(_strafeParameterXZ.x * 100f) / 100f);
        _animator.SetFloat("StrafeZ", Mathf.Round(_strafeParameterXZ.z * 100f) / 100f);
    
        if(_inputs.LockOn.PressedDown())
            _cameraController.ToggleLockOn(!_cameraController.LockedOn);
        
        if (!_inAnimation && _strafing)
        {
            if (_inputs.Attack.PressedDown())
            {
                Debug.Log("Attack Pressed");
                _inAnimation = true;
                _animator.CrossFadeInFixedTime(_animAttack, 0.1f, 0,0);
            }
        }
    }

    private void LateUpdate()
    {
        if(_proning)
            return;
        
        switch (_stance)
        {
            case CharacterStance.Standing:
                if (_inputs.Crouch.PressedDown()){RequestStanceChange(CharacterStance.Crouching);}
                else if (_inputs.Prone.PressedDown()){RequestStanceChange(CharacterStance.Proning);}
                break;
            case CharacterStance.Crouching:
                if (_inputs.Crouch.PressedDown()){RequestStanceChange(CharacterStance.Standing);}
                else if (_inputs.Prone.PressedDown()){RequestStanceChange(CharacterStance.Proning);}
                break;
            case CharacterStance.Proning:
                if (_inputs.Crouch.PressedDown()){RequestStanceChange(CharacterStance.Crouching);}
                else if (_inputs.Prone.PressedDown()){RequestStanceChange(CharacterStance.Standing);}
                break;
        }
    }
    public bool RequestStanceChange(CharacterStance newStance)
    {
        if (_stance == newStance)
            return true;
        switch (_stance)
        {
            case CharacterStance.Standing:
                if (newStance == CharacterStance.Crouching)
                {
                    if (!CharacterOverlap(_crouchingCapsule))
                    {
                        _newSpeed = 0; 
                        _animator.SetFloat(StrafeZ, _newSpeed);
                        _runSpeed = _crouchingSpeed.x;
                        _sprintSpeed = _crouchingSpeed.y;
                        _rotationSharpness = _crouchingRotationSharpness;
                        _stance = newStance;
                        //_swordToggle.enabled = false;
                        _animator.CrossFadeInFixedTime(_standToCrouch, 0.25f);
                        SetCapsuleDimensions(_crouchingCapsule);
                        return true;
                    }
                }
                else if (newStance == CharacterStance.Proning)
                {
                    if (!CharacterOverlap(_proningCapsule))
                    {
                        _newSpeed = 0;
                        _proning = true;
                        _animator.SetFloat(StrafeZ, _newSpeed);
                        
                        _runSpeed = _proningSpeed.x;
                        _sprintSpeed = _proningSpeed.y;
                        _rotationSharpness = _proningRotationSharpness;
                        _stance = newStance;
                        //_swordToggle.enabled = true;
                        _animator.CrossFadeInFixedTime(_standToProne, 0.5f);
                        SetCapsuleDimensions(_proningCapsule);
                        return true;
                    }
                }
                break;
            case CharacterStance.Crouching:
                if (newStance == CharacterStance.Standing)
                {
                    if (!CharacterOverlap(_standingCapsule))
                    {
                        _newSpeed = 0;
                        _animator.SetFloat(StrafeZ, _newSpeed);
                        _runSpeed = _standingSpeed.x;
                        _sprintSpeed = _standingSpeed.y;
                        _rotationSharpness = _standingRotationSharpness;
                        _stance = newStance;
                        //_swordToggle.enabled = true;
                        _animator.CrossFadeInFixedTime(_crouchToStand, 0.25f);
                        SetCapsuleDimensions(_standingCapsule);
                        return true;
                    }
                }
                else if (newStance == CharacterStance.Proning)
                {
                    if (!CharacterOverlap(_proningCapsule))
                    {
                        _newSpeed = 0;
                        _proning = true;
                        _animator.SetFloat(StrafeZ, _newSpeed);
                        
                        _runSpeed = _proningSpeed.x;
                        _sprintSpeed = _proningSpeed.y;
                        _rotationSharpness = _proningRotationSharpness;
                        _stance = newStance;
                        //_swordToggle.enabled = true;
                        _animator.CrossFadeInFixedTime(_crouchToProne, 0.5f);
                        SetCapsuleDimensions(_proningCapsule);
                        return true;
                    }
                }
                break;
            
                case CharacterStance.Proning:
                    if (newStance == CharacterStance.Standing)
                    {
                        if (!CharacterOverlap(_standingCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat(StrafeZ, _newSpeed);
                            
                            _runSpeed = _standingSpeed.x;
                            _sprintSpeed = _standingSpeed.y;
                            _rotationSharpness = _standingRotationSharpness;
                            _stance = newStance;
                            //_swordToggle.enabled = true;
                            _animator.CrossFadeInFixedTime(_proneToStand, 0.25f);
                            SetCapsuleDimensions(_standingCapsule);
                            return true;
                        }
                    }
                    else if (newStance == CharacterStance.Crouching)
                    {
                        if (!CharacterOverlap(_crouchingCapsule))
                        {
                            _newSpeed = 0;
                            _proning = true;
                            _animator.SetFloat(StrafeZ, _newSpeed);
                            
                            _runSpeed = _crouchingSpeed.x;
                            _sprintSpeed = _crouchingSpeed.y;
                            _rotationSharpness = _crouchingRotationSharpness;
                            _stance = newStance;
                            //_swordToggle.enabled = false;
                            _animator.CrossFadeInFixedTime(_proneToCrouch, 0.25f);
                            SetCapsuleDimensions(_crouchingCapsule);
                            return true;
                        }
                    }
                break;
        }
        return false;
    }
    private bool CharacterOverlap(Vector3 dimensions)
    {
        float _radius = dimensions.x;
        float _height = dimensions.y;
        Vector3 _center = new Vector3(_collider.center.x, dimensions.z, _collider.center.z);

        Vector3 _point0;
        Vector3 _point1;
        if (_height < _radius * 2)
        {
            _point0 = transform.position + _center;
            _point1 = transform.position + _center;
        }
        else
        {
            _point0 = transform.position + _center + (transform.up * (_height * 0.5f - _radius));
            _point1 = transform.position + _center + (transform.up * (_height * 0.5f - _radius));
        }
        
        int _numOverlaps = Physics.OverlapCapsuleNonAlloc(_point0, _point1, _radius, _obstructions, _layerMask);
        for (int i = 0; i < _numOverlaps; i++)
        {
            if (_obstructions[i] == _collider)
                _numOverlaps--;
        }

        return _numOverlaps > 0;
    }
    private void SetCapsuleDimensions(Vector3 dimensions)
    {
        _collider.center = new Vector3(_collider.center.x, dimensions.z, _collider.center.z);
        _collider.radius = dimensions.x;
        _collider.height = dimensions.y;
    }
    private void OnAnimatorMove()
    {
        if (_inAnimation)
        {
            _animatorVelocity = _animator.velocity;
            _animatorDeltaRotation = _animator.deltaRotation;
        }
    }
    public void OnSMBEvent(string eventName)
    {
        switch (eventName)
        {
            case "ProneEnd":
                _proning = false;
                break;
        }
    }
}
