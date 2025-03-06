using UnityEngine;

public class PortalCameraManager : MonoBehaviour
{
    [Header("Plane 1 Cameras")]
    [SerializeField] private Camera towardsKitchenCamera1;
    [SerializeField] private Camera towardsEndCamera;
    [Header("Kitchen Cameras")]
    [SerializeField] private Camera towardsPlane1Camera1;
    [SerializeField] private Camera towardsPlane2Camera1;
    [Header("Plane 2 Cameras")]
    [SerializeField] private Camera towardsKitchenCamera2;
    [SerializeField] private Camera towardsAppCamera1;
    [Header("App Cameras")]
    [SerializeField] private Camera towardsPlane2Camera2;
    [SerializeField] private Camera towardsKeyCamera;
    [Header("Key Camera")]
    [SerializeField] private Camera towardsAppCamera2;
    [Header("End Camera")]
    [SerializeField] private Camera towardsPlane1Camera2;
    [Header("Current Player Location")]
    [SerializeField] private PlayerLocation currentPlayerLocation;
    public enum PlayerLocation
    {
        Plane1,
        Kitchen,
        Plane2,
        App1st,
        App2ndto4th,
        App5th,
        Key,
        End
    }
    
    private void Start()
    {
        UpdateActiveCameras();
    }

    public void UpdatePlayerLocation(PlayerLocation newLocation)
    {
        currentPlayerLocation = newLocation;
        UpdateActiveCameras();
    }

    private void UpdateActiveCameras()
    {
        towardsAppCamera1.enabled = false;
        towardsAppCamera2.enabled = false;
        towardsEndCamera.enabled = false;
        towardsKeyCamera.enabled = false;
        towardsKitchenCamera1.enabled = false;
        towardsKitchenCamera2.enabled = false;
        towardsPlane1Camera1.enabled = false;
        towardsPlane1Camera2.enabled = false;
        towardsPlane2Camera1.enabled = false;
        towardsPlane2Camera2.enabled = false;

        switch (currentPlayerLocation)
        {
            case PlayerLocation.Plane1:
                towardsKitchenCamera1.enabled = true;
                towardsEndCamera.enabled = true;
                break;
            case PlayerLocation.Kitchen:
                towardsPlane1Camera1.enabled = true;
                towardsPlane2Camera1.enabled = true;
                break;
            case PlayerLocation.Plane2:
                towardsKitchenCamera2.enabled = true;
                towardsAppCamera1.enabled = true;
                break;
            case PlayerLocation.App1st:
                towardsPlane2Camera2.enabled = true;
                break;
            case PlayerLocation.App2ndto4th:
                break;
            case PlayerLocation.App5th:
                towardsKeyCamera.enabled = true;
                break;
            case PlayerLocation.Key:
                towardsAppCamera2.enabled = true;
                break;
            case PlayerLocation.End:
                towardsPlane1Camera2.enabled = true;
                break;
        }
    }
}
