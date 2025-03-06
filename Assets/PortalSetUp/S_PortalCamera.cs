using System;
using UnityEngine;

public class S_PortalCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Camera portalCamera;
    [SerializeField] private Transform portal;
    [SerializeField] private Transform linkedPortal;
    [Header("Important Setting")]
    [SerializeField] private float offset;

    private void Start()
    {
        if (playerCamera != null) return;
        if (Camera.main != null) playerCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        RotatePortalCamera();
        ChangePortalCameraFOV();
    }

    private void RotatePortalCamera()
    {
        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, linkedPortal.rotation) + offset;

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }

    private void ChangePortalCameraFOV()
    {
        float distance = Vector3.Distance(playerCamera.position, linkedPortal.position);
        float t = Mathf.Clamp01(distance / 10);
        portalCamera.fieldOfView = Mathf.Lerp(60, 90, t);
    }
}
