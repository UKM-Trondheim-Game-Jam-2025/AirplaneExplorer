using System;
using UnityEngine;

public class S_Teleportation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform receiver;
    
    private Rigidbody _playerRb;
    private PlayerController _playerController;
    
    private bool _isOverlapping = false;
    
    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>().transform;
        }
        _playerRb = player.GetComponent<Rigidbody>();
        _playerController = player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isOverlapping = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isOverlapping = false;
        }
    }

    private void LateUpdate()
    {
        if (_isOverlapping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if (dotProduct < 0f)
            {
                float rotationDifference = -Quaternion.Angle(transform.rotation, receiver.rotation);
                rotationDifference += 180;
                player.Rotate(Vector3.up, rotationDifference);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDifference, 0f) * portalToPlayer;
                player.position = receiver.position + positionOffset;
                
                _isOverlapping = false;
            }
        }
    }
}
