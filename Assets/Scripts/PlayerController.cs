using System.Collections;
using UnityEngine;
using InputHandling;
using MovementHandling;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
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
    [SerializeField] CeilingDetector ceilingDetector; // Reference to our new ceiling detector
    CapsuleCollider m_Collider;
    float m_StartColliderHeight;

    [Header("Input")]
    [SerializeField] InputReader inputReader;
    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouching;
    [SerializeField] bool wantsToCrouch = false;
    [SerializeField] bool forcedCrouching = false; // New field to track if crouching is forced by ceiling
    [SerializeField] Vector2 moveInput;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] bool isGrounded;

    [SerializeField] Transform orientation;
    
    [Header("Teleporting")]
    [SerializeField] bool isTeleporting = false;
    public bool IsTeleporting => isTeleporting;

    MovementStateHandler m_StateChain;
    Rigidbody m_Rigidbody;

    float m_MovementSpeed;

    // Properties for handler access
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public bool IsCrouching => isCrouching;
    public float CrouchSpeed => crouchSpeed;
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;

    void OnEnable()
    {
        if (inputReader is null) return;
        inputReader.MoveEvent += OnMove;
        inputReader.SprintEvent += OnSprint;
        inputReader.JumpEvent += OnJump;
        inputReader.CrouchEvent += OnCrouch;
        inputReader.EnablePlayerActions();

        // Subscribe to ceiling detector events
        if (ceilingDetector is not null)
            ceilingDetector.OnCeilingStateChanged += OnCeilingStateChanged;
    }

    void OnDisable()
    {
        if (inputReader is null) return;
        inputReader.MoveEvent -= OnMove;
        inputReader.SprintEvent -= OnSprint;
        inputReader.JumpEvent -= OnJump;
        inputReader.CrouchEvent -= OnCrouch;
        inputReader.DisablePlayerActions();

        // Unsubscribe from ceiling detector events
        if (ceilingDetector is not null)
            ceilingDetector.OnCeilingStateChanged -= OnCeilingStateChanged;
    }

    void Start()
    {
        startYScale = transform.localScale.y;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.freezeRotation = true;
        canJump = true;

        // Get and store the collider reference and height
        m_Collider = GetComponent<CapsuleCollider>();
        m_StartColliderHeight = m_Collider.height;

        // Setup chain of responsibility
        InitializeStateHandlers();

        // Initialize ceiling detector if not assigned in inspector
        if (ceilingDetector == null)
        {
            // Try to find an existing ceiling detector child
            ceilingDetector = GetComponentInChildren<CeilingDetector>();

            // If none found, create one
            if (ceilingDetector == null)
            {
                GameObject detectorObj = new GameObject("CeilingDetector");
                detectorObj.transform.SetParent(transform);
                detectorObj.transform.localPosition = new Vector3(0, m_Collider.height * 0.5f, 0); // Position at head level

                ceilingDetector = detectorObj.AddComponent<CeilingDetector>();
                // Set up detector properties
                var boxCollider = ceilingDetector.GetComponent<BoxCollider>();
                boxCollider.size = new Vector3(m_Collider.radius * 1.8f, 0.2f, m_Collider.radius * 1.8f);

                // Set layer mask to match the ground layer (assuming ceiling and ground use the same layer)
                ceilingDetector.gameObject.AddComponent<BoxCollider>().isTrigger = true;

                // Subscribe to the event
                ceilingDetector.OnCeilingStateChanged += OnCeilingStateChanged;

                Debug.Log("Created ceiling detector");
            }
        }

        // Setup the detector's layer mask to match the ground layer mask
        // This will require getting the serialized field via reflection or setting in inspector
    }

    void InitializeStateHandlers()
    {
        var crouchingHandler = new CrouchingStateHandler();
        var sprintingHandler = new SprintingStateHandler();
        var walkingHandler = new WalkingStateHandler();
        var airHandler = new AirStateHandler();

        crouchingHandler.SetNext(sprintingHandler);
        sprintingHandler.SetNext(walkingHandler);
        walkingHandler.SetNext(airHandler);

        m_StateChain = crouchingHandler;
    }

    void Update()
    {
        // Check the ground to see if the Player is colliding with it.
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);

        // Handle crouching state
        HandleCrouchingState();

        // Use chain of responsibility for state handling
        m_StateChain.HandleState(this);

        BalanceSpeed();

        // Balances the unnecessary linear dragging when the Player is in motion.
        m_Rigidbody.linearDamping = isGrounded ? groundDrag : 0;
    }

    public void FixedUpdate()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        // Project movement vectors to horizontal plane using Vector3.ProjectOnPlane
        var right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
        var forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;

        // Calculate movement direction
        var movement = right * moveInput.x + forward * moveInput.y;

        // Apply force based on grounded state
        float forceMultiplier = isGrounded ? 10f : 10f * airMultiplier;
        m_Rigidbody.AddForce(movement.normalized * (m_MovementSpeed * forceMultiplier), ForceMode.Force);
    }
    

    /// <summary>
    /// Initiates a teleportation cooldown of 3 seconds
    /// </summary>
    public void Teleporting()
    {
        if (!isTeleporting)
        {
            StartCoroutine(TeleportingCooldown());
        }
    }

    /// <summary>
    /// Coroutine that handles the teleportation cooldown
    /// </summary>
    IEnumerator TeleportingCooldown()
    {
        isTeleporting = true;
    
        if (Debug.isDebugBuild)
            Debug.Log("Teleporting cooldown started");
        
        yield return new WaitForSeconds(3f);
    
        isTeleporting = false;
    
        if (Debug.isDebugBuild)
            Debug.Log("Teleporting cooldown ended");
    }

#region Movement Event Methods
    void OnMove(Vector2 input)
    {
        moveInput = input;
    }
    public void SetMovementSpeed(float speed)
    {
        m_MovementSpeed = speed;
    }

    void OnJump(bool isPressed)
    {
        if (!isPressed || !canJump || !isGrounded)
            return;
        canJump = false;
        Jump();
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    void OnSprint(bool sprinting)
    {
        isSprinting = sprinting;
    }

    void OnCrouch(bool isPressed)
    {
        // Only track the player's intention to crouch
        wantsToCrouch = isPressed;

        // Add a small force when initiating a crouch
        if (isPressed && !isCrouching && isGrounded)
        {
            m_Rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Actual crouch state and transform changes are handled in HandleCrouchingState
    }

    // New method to handle ceiling detection events
    void OnCeilingStateChanged(bool ceilingDetected)
    {
        forcedCrouching = ceilingDetected;

        // Force update crouching state immediately
        HandleCrouchingState();

        if (Debug.isDebugBuild)
            Debug.Log($"Ceiling detection changed: {ceilingDetected}");
    }

    // Modified HandleCrouchingState to use the ceiling detector
    void HandleCrouchingState()
    {
        // Determine crouch state:
        // - User wants to crouch, OR
        // - Force crouch when ceiling is detected (regardless of previous state)
        bool shouldCrouch = wantsToCrouch || forcedCrouching;

        // Only update state and scale if there's a change
        if (shouldCrouch == isCrouching)
            return;

        // Debug output for visibility
        if (Debug.isDebugBuild && forcedCrouching)
            Debug.Log("Ceiling detected above player!");

        isCrouching = shouldCrouch;

        // Use hardcoded values if the configured values are identical
        float effectiveStartYScale = startYScale;
        float effectiveCrouchYScale = crouchYScale;

        // If the two scales are the same, force them to be different
        if (Mathf.Approximately(effectiveStartYScale, effectiveCrouchYScale))
        {
            effectiveCrouchYScale = effectiveStartYScale * 0.5f; // Set crouch to half height
            Debug.LogWarning("Crouch and start scales are identical. Using half height for crouch.");
        }

        // Set scale based on crouch state
        float targetYScale = isCrouching ? effectiveCrouchYScale : effectiveStartYScale;
        transform.localScale = new Vector3(transform.localScale.x, targetYScale, transform.localScale.z);

        // Set collider height directly
        float targetColliderHeight = isCrouching ? m_StartColliderHeight * 0.5f : m_StartColliderHeight;
        m_Collider.height = targetColliderHeight;

        // Adjust collider center to keep feet at same position
        float centerYOffset = (m_StartColliderHeight - targetColliderHeight) / 2;
        m_Collider.center = new Vector3(
            m_Collider.center.x,
            isCrouching ? centerYOffset : 0,
            m_Collider.center.z);

        // Update ceiling detector position to match new height
        if (ceilingDetector != null)
        {
            // Position the detector at the player's head level
            ceilingDetector.transform.localPosition = new Vector3(
                0,
                m_Collider.height * 0.5f - m_Collider.center.y,
                0
            );
        }

        if (Debug.isDebugBuild)
            Debug.Log($"Crouch: {isCrouching}, Ceiling Above: {forcedCrouching}, Scale: {targetYScale}, Collider Height: {m_Collider.height}");
    }

    void BalanceSpeed()
    {
        var flatVelocity = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);

        if (flatVelocity.magnitude <= m_MovementSpeed)
            return;

        var limitVelocity = flatVelocity.normalized * m_MovementSpeed;
        m_Rigidbody.linearVelocity = new Vector3(limitVelocity.x, m_Rigidbody.linearVelocity.y, limitVelocity.z);
    }

    void Jump()
    {
        m_Rigidbody.linearVelocity = new Vector3(m_Rigidbody.linearVelocity.x, 0f, m_Rigidbody.linearVelocity.z);
        m_Rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Player has jumped");
    }

    void ResetJump()
    {
        canJump = true;
    }
#endregion
}