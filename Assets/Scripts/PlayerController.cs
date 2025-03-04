using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TMPro;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 15.0f;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool _canJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayerMask;
    public bool _isGrounded;

    [SerializeField]
    private Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Rigidbody rb;

    public TextMeshProUGUI textBoxField;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        _canJump = true;
    }
    void Update()
    {
        // Check the ground to see if the Player is colliding with it.
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);

        MovementInput();
        BalanceSpeed();

        // Balances the unnecessary linear dragging when the Player is in motion.
        if (_isGrounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0; 
        BalanceSpeed();
    }
    public void FixedUpdate()
    {
        PlayerMovement();
    }
    private void MovementInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Checks if the Player can jump
        if (Input.GetKey(jumpKey) && _canJump && _isGrounded)
        {
            Jump();

            _canJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void PlayerMovement()
    {
        // Calculates the Movement Direction
        Vector3 movement = orientation.right * horizontalInput + orientation.forward * verticalInput;
        
        // Will allow the Player to move if they're on the ground.
        if(_isGrounded)
        rb.AddForce(movement.normalized * movementSpeed * 10f, ForceMode.Force);

        // Will not allow the Player to move if they're in the air.
        else if (!_isGrounded)
        rb.AddForce(movement.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void BalanceSpeed()
    {
        Vector3 flatVelovity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelovity.magnitude > movementSpeed)
        {
            Vector3 limitVelocity = flatVelovity.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);

            textBoxField.SetText("Speed: " + flatVelovity.magnitude);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        print("Player has jumped");

    }

    private void ResetJump()
    {
        _canJump = true;
    }
}
