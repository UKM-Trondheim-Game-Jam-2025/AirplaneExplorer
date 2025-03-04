using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 15.0f;

    [SerializeField]
    private float groundDrag;

    public float playerHeight;
    public LayerMask groundLayerMask;
    public bool _isGrounded;

    [SerializeField]
    private Transform orientation;

    private Rigidbody rb;

    public TextMeshProUGUI textBoxField;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = orientation.right * horizontal + orientation.forward * vertical;

        rb.AddForce(movement.normalized * (movementSpeed * 10f), ForceMode.Force);

        if (_isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0; 
        }
        BalanceSpeed();
    }

    public void BalanceSpeed()
    {
        Vector3 flatVelovity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelovity.magnitude > movementSpeed)
        {
            Vector3 limitVelocity = flatVelovity.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);

            textBoxField.SetText("Speed: " + flatVelovity.magnitude);
        }
    }
}
