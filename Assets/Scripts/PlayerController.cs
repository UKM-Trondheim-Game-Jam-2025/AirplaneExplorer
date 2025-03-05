using UnityEngine;
    using TMPro;
    using InputHandling;
    using UnityEngine.Serialization;

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float movementSpeed;
        [SerializeField] float walkSpeed;
        [SerializeField] float sprintSpeed;
    
        [SerializeField] float groundDrag;
    
        [Header("Jumping")]
        [SerializeField] float jumpForce;
        [SerializeField] float jumpCooldown;
        [SerializeField] float airMultiplier;
        [SerializeField] bool canJump;
    
        [Header("Crouching")]
        [SerializeField] float crouchSpeed;
        [SerializeField] float crouchYScale;
        [SerializeField] float startYScale;
    
        [Header("Input")]
        [SerializeField] InputReader inputReader;
        [SerializeField] bool isSprinting;
        [SerializeField] bool isCrouching;
        [SerializeField] Vector2 moveInput;
    
        [Header("Ground Check")]
        [SerializeField] float playerHeight;
        [SerializeField] LayerMask groundLayerMask;
        [SerializeField] bool isGrounded;
    
        [SerializeField] Transform orientation;
    
        Rigidbody m_Rigidbody;
    
        public TextMeshProUGUI TextBoxField;
    
        public MovementState State;
        public enum MovementState
        {
            Walking,
            Sprinting,
            Crouching,
            Air
        }
    
        void OnEnable()
        {
            if (inputReader is null) return;
            inputReader.MoveEvent += OnMove;
            inputReader.SprintEvent += OnSprint;
            inputReader.JumpEvent += OnJump;
            inputReader.CrouchEvent += OnCrouch;
            inputReader.EnablePlayerActions();
        }
    
        void OnDisable()
        {
            if (inputReader is null) return;
            inputReader.MoveEvent -= OnMove;
            inputReader.SprintEvent -= OnSprint;
            inputReader.JumpEvent -= OnJump;
            inputReader.CrouchEvent -= OnCrouch;
            inputReader.DisablePlayerActions();
        }
    
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.freezeRotation = true;
    
            canJump = true;
    
            startYScale = transform.localScale.y;
        }
    
        void Update()
        {
            // Check the ground to see if the Player is colliding with it.
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
    
            StateHandling();
            BalanceSpeed();
    
            // Balances the unnecessary linear dragging when the Player is in motion.
            if (isGrounded)
                m_Rigidbody.linearDamping = groundDrag;
            else
                m_Rigidbody.linearDamping = 0;
        }
    
        public void FixedUpdate()
        {
            PlayerMovement();
        }
        
        void PlayerMovement()
        {
            // Project movement vectors to horizontal plane using Vector3.ProjectOnPlane
            Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
            Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
            
            // Calculate movement direction
            Vector3 movement = right * moveInput.x + forward * moveInput.y;

            // Apply force based on grounded state
            if (isGrounded)
            {
                m_Rigidbody.AddForce(movement.normalized * (movementSpeed * 10f), ForceMode.Force);
            }
            else
            {
                m_Rigidbody.AddForce(movement.normalized * (movementSpeed * 10f * airMultiplier), ForceMode.Force);
            }
        }
    
        void OnMove(Vector2 input)
        {
            moveInput = input;
        }
    
        void OnJump(bool isPressed)
        {
            if (isPressed && canJump && isGrounded)
            {
                canJump = false;
    
                Jump();
    
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    
        // This method would be called when sprint input is implemented
        void OnSprint(bool sprinting)
        {
            isSprinting = sprinting;
        }
    
        // This method would be called when crouch input is implemented
        void OnCrouch(bool isPressed)
        {
            if (isPressed)
            {
                // Start crouching
                isCrouching = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                m_Rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            else
            {
                // Stop crouching
                isCrouching = false;
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            }
        }
    
        void StateHandling()
        {
            // Movement State - Crouching
            if (isCrouching)
            {
                State = MovementState.Crouching;
                movementSpeed = crouchSpeed;
            }
            // Movement State - Sprinting
            else if (isGrounded && isSprinting)
            {
                State = MovementState.Sprinting;
                movementSpeed = sprintSpeed;
            }
            // Movement State - Walking
            else if (isGrounded)
            {
                State = MovementState.Walking;
                movementSpeed = walkSpeed;
            }
            // Movement State - Air
            else
            {
                State = MovementState.Air;
            }
        }
        
        void BalanceSpeed()
        {
            var flatVelovity = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);

            if (flatVelovity.magnitude > movementSpeed)
            {
                Vector3 limitVelocity = flatVelovity.normalized * movementSpeed;
                m_Rigidbody.linearVelocity = new Vector3(limitVelocity.x, m_Rigidbody.linearVelocity.y, limitVelocity.z);
            }
    
            TextBoxField.SetText("Speed: " + flatVelovity.magnitude);
        }
    
        void Jump()
        {
            m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);
    
            m_Rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            print("Player has jumped");
    
        }
    
        void ResetJump()
        {
            canJump = true;
        }
    }