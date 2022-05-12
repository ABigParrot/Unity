using System.Collections.Generic;
using UnityEngine;


public class Comp_CameraController : MonoBehaviour
{
    [Header("Framing")]
    [SerializeField] private Camera _camera = null;
    [SerializeField] private Transform _followTransform = null;
    [SerializeField] private Vector3 _framing = new Vector3(0, 0, 0);

    [Header("Distance")] 
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _defaultDistance = 5f;
    [SerializeField] private float _minDistance = 0f;
    [SerializeField] private float _maxDistance = 10f;

    [Header("Rotation")] 
    [SerializeField] private float _minVerticalAngle = -90;
    [SerializeField] private float _maxVerticalAngle = 90;
    [SerializeField] private float _defaultVerticalAngle = 20f;
    [SerializeField] private float _rotationSharpness = 25f;
    [SerializeField] private bool _invertX = false;
    [SerializeField] private bool _invertY = false;

    [Header("Obstructions")] 
    [SerializeField] private float _checkRadius = 0.2f;
    [SerializeField] private LayerMask _obstructionLayers = -1;
    private List<Collider> _ignoreColliders = new List<Collider>();

    [Header("Lock On")] 
    [SerializeField] private float _lockOnDistance = 10f;
    [SerializeField] private float _lockOnLossTime = 1f;
    [SerializeField] private LayerMask _lockOnLayers = -1;


    public bool LockedOn { get => _lockedOn; }
    public ITargetable Target { get => _target; }

    public Vector3 CameraPlanarDirection {get => _planarDirection; }

    //Privates
    public Vector3 _planarDirection; //Cameras forward on the x,z plane
    private Vector3 _targetPosition;
    private float _targetDistance;
    private Quaternion _targetRotation;
    private float _targetVerticalAngle;

    private Vector3 _newPosition;
    private Quaternion _newRotation;

    private bool _lockedOn;
    private float _lockOnLossTimeCurrent;
    private ITargetable _target;

    private void OnValidate()
    {
        _defaultDistance = Mathf.Clamp(_defaultDistance, _minDistance, _maxDistance);
        _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
    
    }

    private void Start()
    {
        _ignoreColliders.AddRange(GetComponentsInChildren<Collider>()); 
        //Fixes the camera on the forward of the players forward direction
        _planarDirection = _followTransform.forward;

        //Calculate targets
        _targetDistance = _defaultDistance;
        _targetVerticalAngle = _defaultVerticalAngle;
    
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        //Handle inputs
        //First time I referenced a custom-made class. This references the Mouse X and Y inputs
        float _MouseX = Comp_PlayerInput.MouseXInput;
        float _MouseY = Comp_PlayerInput.MouseYInput;
        float _zoom = Comp_PlayerInput.MouseScrollInput * _zoomSpeed;
    
        if(_invertX) {_MouseX *= -1f;}
        if(_invertY) {_MouseY *= -1f;}

        Vector3 _focusPosition = _followTransform.position + _camera.transform.TransformDirection(_framing);

        //Locked on conditional
        if (_lockedOn && _target != null)
        {
            Vector3 _camToTarget = _target.TargetTransform.position - _camera.transform.position;
            Vector3 _planarCamToTarget = Vector3.ProjectOnPlane(_camToTarget, Vector3.up);
            Quaternion _lookRotation = Quaternion.LookRotation(_camToTarget, Vector3.up);

            _planarDirection = _planarCamToTarget != Vector3.zero ? _planarCamToTarget.normalized : _planarDirection;
            _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance);
            _targetVerticalAngle = Mathf.Clamp(_lookRotation.eulerAngles.x, _minVerticalAngle, _maxVerticalAngle);
        }
        else
        {
        
            //Puts MouseX input into the Y-Axis (rotation around the Y axis) and multiplies it by the planar direction so player can look left and right by moving the mouse left or right
            _planarDirection = Quaternion.Euler(0, _MouseX, 0) * _planarDirection;
            //Limits the distance between the camera and the player
            _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance);
            /*
            Limits the vertical angle; 
                adds the vertical angle by the Mouse Y input, 
                and limits the viewing angle between the min and max angles.
                Apply a minus sign infront of the _MouseY value in the clamp 
                function to make the player look up when the mouse moves forward
         */
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle + _MouseY, _minVerticalAngle, _maxVerticalAngle);
        }

        float _smallestDistance = _targetDistance;
        RaycastHit[] _hits = Physics.SphereCastAll(
            _focusPosition, 
            _checkRadius, 
            _targetRotation * -Vector3.forward,
            _targetDistance, 
            _obstructionLayers
        );
        if(_hits.Length != 0)
            foreach(RaycastHit hit in _hits)
                if(!_ignoreColliders.Contains(hit.collider))
                    if (hit.distance < _smallestDistance)
                        _smallestDistance = hit.distance;
    
    
        //Final Targets; applies the calculations from the planar direction and multiplies that with the target vertical angle as an x-value Euler angle.
        _targetRotation = Quaternion.LookRotation(_planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
        _targetPosition = _focusPosition - (_targetRotation * Vector3.forward) * _smallestDistance;
    
        _newRotation = Quaternion.Slerp(_camera.transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
        _newPosition = Vector3.Lerp(_camera.transform.position, _targetPosition, Time.deltaTime * _rotationSharpness);
    
        //Apply
        _camera.transform.rotation = _newRotation;
        _camera.transform.position =  _newPosition;

        if (_lockedOn && _target != null)
        {
            bool _valid =
                _target.Targetable &&
                InDistance(_target) &&
                InScreen(_target) &&
                NotBlocked(_target);
        
            if(_valid){_lockOnLossTimeCurrent = 0;}
            else      {_lockOnLossTimeCurrent = Mathf.Clamp(_lockOnLossTimeCurrent + Time.deltaTime, 0, _lockOnLossTime);}

            if (_lockOnLossTimeCurrent == _lockOnLossTime)
                _lockedOn = false;
        }
    }

    public void ToggleLockOn(bool toggle)
    {
        //Early out
        if (toggle == _lockedOn)
            return;
    
        //Toggle
        _lockedOn = !_lockedOn;

        if (_lockedOn)
        {
            //Filter targetables
            List<ITargetable> _targetables = new List<ITargetable>();
            Collider[] _colliders = Physics.OverlapSphere(transform.position, _lockOnDistance, _lockOnLayers);
            foreach (Collider _collider in _colliders)
            {
                //Gets the interface
                ITargetable _targetable = _collider.GetComponent<ITargetable>();
                //If the target isn't null
                if (_targetable != null)
                    //If the targetable is true
                    if (_targetable.Targetable)
                        //If the target is on screen
                        if (InScreen(_targetable))
                            //If the target is not blocked by an obstacle
                            if(NotBlocked(_targetable))
                                _targetables.Add(_targetable);
            }
        
            //Find closest hypotenuse
            float _hypotenuse;
            float _smallestHypotenuse = Mathf.Infinity;
            ITargetable _closestTargetable = null;

            foreach (ITargetable _targetable in _targetables)
            {
                _hypotenuse = CalculateHypotenuse(_targetable.TargetTransform.position);
                if (_smallestHypotenuse > _hypotenuse)
                {
                    _closestTargetable = _targetable;
                    _smallestHypotenuse = _hypotenuse;
                }
            }
        
            //Final
            _target = _closestTargetable;
            _lockedOn = _closestTargetable != null;
        }
    }
    private bool InDistance(ITargetable _targetable)
    {
        float _distance = Vector3.Distance(transform.position, _targetable.TargetTransform.position);
        return _distance <= _lockOnDistance;
    }
    private bool InScreen(ITargetable targetable)
    {
        Vector3 _viewPortPosition = _camera.WorldToViewportPoint(targetable.TargetTransform.position);
    
        //if the target is not within the viewport boundary, return false. If the target is within viewport boundary, return true.
        if(!(_viewPortPosition.x > 0) || !(_viewPortPosition.x < 1)){return false;}
        if(!(_viewPortPosition.y > 0) || !(_viewPortPosition.y < 1)){return false;}
        if(!(_viewPortPosition.z > 0))                              {return false;}

        return true;
    }
    private bool NotBlocked(ITargetable targetable)
    {
        Vector3 _origin = _camera.transform.position;
        Vector3 _direction = targetable.TargetTransform.position - _origin;

        float _radius = 0.15f;
        float _distance = _direction.magnitude;
        bool _notBlocked = !Physics.SphereCast(_origin, _radius, _direction, out RaycastHit _hit, _distance, _obstructionLayers);

        return _notBlocked;
    }
    private float CalculateHypotenuse(Vector3 position)
    {
        float _screenCenterX = _camera.pixelWidth / 2;
        float _screenCenterY = _camera.pixelHeight / 2;

        Vector3 _screenPosition = _camera.WorldToScreenPoint(position);
        float _xDelta = _screenCenterX - _screenPosition.x;
        float _yDelta = _screenCenterY - _screenPosition.y;
        float _hypotenuse = Mathf.Sqrt(Mathf.Pow(_xDelta, 2) + Mathf.Pow(_yDelta, 2));

        return _hypotenuse;
    }
}
