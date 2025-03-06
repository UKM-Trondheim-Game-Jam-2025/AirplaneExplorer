using UnityEngine;

public class PlayerLocationTrigger: MonoBehaviour
{
    [SerializeField] private PortalCameraManager portalCameraManager;
    [SerializeField] private PortalCameraManager.PlayerLocation newLocation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            portalCameraManager.UpdatePlayerLocation(newLocation);
        }
    }
}
