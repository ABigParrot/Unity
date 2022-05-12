using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    private Camera _mainCamera;
    private PlayerControls _controls;
    private CharacterController _controller;
    public Animator animator;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    
    private float _groundedTimer;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float turnSpeed = 12f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector2 mouseInput;
    [SerializeField] private Vector3 animatorInput;
    [SerializeField] private Vector3 moveInput;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed = 3f;
    
    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;
    
    void Awake()
    {
        //Locks the cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;

        //Store the main camera in a variable for later use
        _mainCamera = Camera.main;
        
        //Store the player controls in a variable
        _controls = new PlayerControls();

        //movement keys used. This is an event
        _controls.KeyBoard.WASDKeys.performed += ctx =>
        {
            moveInput = new Vector3(ctx.ReadValue<Vector2>().x, moveInput.y, ctx.ReadValue<Vector2>().y);
        };
        
        //Movement keys no longer used. This is an event
        _controls.KeyBoard.WASDKeys.canceled += ctx =>
        {
            Debug.Log("WASD key unpressed");
            moveInput = new Vector3(ctx.ReadValue<Vector2>().x, moveInput.y, ctx.ReadValue<Vector2>().y);
        };

 
        //Gets the input for jumping. This is an event
        _controls.KeyBoard.Jump.performed += ctx =>
        {
            Debug.Log("Space bar pressed");
            if (isGrounded && velocity.y < 0)
            {
                animator.SetBool("IsJumping", true);
                velocity.y = (float) Math.Sqrt(jumpHeight * -2f * gravity);
            }
        };
        //Spacebar released. This is an event
        _controls.KeyBoard.Jump.canceled += ctx =>
        {
            Debug.Log("Space bar unpressed");
        };

        //Mouse horizontal and vertical input. This is an event
        _controls.KeyBoard.MouseDelta.performed += ctx =>
        {
            //Store mouse input into a variable for later use
            mouseInput = ctx.ReadValue<Vector2>();
        };
            
        //Store the controller in a variable
        _controller = GetComponent<CharacterController>();
        
        //Store the animator in a variable
        animator = GetComponent<Animator>();
        
        
    }

    //Invokes this every frame
    void Update()
    {
        float zInput = moveInput.z;

        //Velocity at the Y-Axis
        velocity.y += gravity * Time.deltaTime;
        
        //Boolean value for checking if the player is grounded
        bool groundedPlayer = _controller.isGrounded;
        
   
        if (groundedPlayer)
        {
            _groundedTimer = 0.02f;
        }

        if (_groundedTimer > 0)
        {
            _groundedTimer -= Time.deltaTime;
        }

        //This checks if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
    
        //If the player is grounded and the falling speed is a lot, reset the velocity at the y-axis to keep player grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsJumping", false);
        }

        if ((zInput == 1f && velocity.y < -2f) || (zInput == -1f && velocity.y < -2f))
        {
            isGrounded = true;
        }

        
    }

    private void LateUpdate()
    {
        Move();
        Look();
    }

    /// <summary>
    /// Moves the character and plays running animations according to the input
    /// </summary>
    void Move()
    {
 
        Vector3 moveVec = transform.right * moveInput.x + transform.forward * moveInput.z;
        
        _controller.Move((moveVec) * speed * Time.deltaTime);
        _controller.Move(velocity * Time.deltaTime);
    
        
        animatorInput.x = moveInput.x;
        animatorInput.z = moveInput.z;
        
        
        //SetFloat takes these parameters: name, value, dampTime, deltaTime. 
        //For keyboard input, ALWAYS add dampTime and deltaTime to the animator float values
        //in your 2D blendtrees. 
        //Do the same for joystick controls for now, unless proven otherwise.
        animator.SetFloat("InputX", animatorInput.x, 1f, Time.deltaTime * 10f);
        animator.SetFloat("InputY", animatorInput.z, 1f, Time.deltaTime * 10f);

    }
    

    /// <summary>
    /// Makes the character face the direction the camera is looking
    /// </summary>
    void Look()
    {
       //Plug into the y axis for the 
        float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,yawCamera,0),turnSpeed * Time.deltaTime);

        
    }
/*
    private bool OnSlope()
    {
        if (_controls.KeyBoard.Jump.triggered)
            return false;
        
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, ))
        
        return false;
    }
    */
    //Enables and disables the controls. NEEDED.
    private void OnEnable()
    {
        _controls.Enable();
    }
       private void OnDisable()
    {
        _controls.Disable();
    }

  
}

     
