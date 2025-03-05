using System;
using Unity.Cinemachine;
using UnityEngine;

public class S_Teleportation : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform connectedPortalCamera;
    
    private Rigidbody _playerRb;
    private PlayerController _playerController;
    
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
            _playerRb.linearVelocity = Vector3.zero;
            Vector3 portalToPlayer = player.position - transform.position;
            float rotationDifference = -Quaternion.Angle(transform.rotation, connectedPortalCamera.rotation);
            rotationDifference += 180;
            player.GetChild(2).GetComponent<CinemachinePanTilt>().PanAxis.Value = connectedPortalCamera.rotation.eulerAngles.y;
            
            Vector3 positionOffset = Quaternion.Euler(0f, rotationDifference, 0f) * portalToPlayer;
            
            player.position = connectedPortalCamera.position + positionOffset;
        }
    }
}
