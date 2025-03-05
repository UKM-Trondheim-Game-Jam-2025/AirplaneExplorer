using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour, IDataPersistance
{
    [Header("Player References")]
    [SerializeField] public Transform hand;
    [SerializeField] private Camera playerCamera;
    [Header("Interaction Data")]
    [SerializeField] float interactionDistance;
    private IInteractable currentInteractable;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        PerformRaycast();
        HandleInteract();
    }

    void PerformRaycast()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();

            if (interactable != null && interactable != currentInteractable)
            {
                currentInteractable = interactable;
                Debug.Log("Interaction Object = " + currentInteractable);
            }
        }
        else
        {
            currentInteractable = null;
        }
    }

    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = this.transform.position;
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null) return;
        Gizmos.color = Color.green;
        Ray rayc = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.DrawRay(rayc.origin, rayc.direction * interactionDistance);
    }

    private void HandleInteract()
    {
        switch (currentInteractable)
        {
            case null:
            {
                if (hand.childCount > 0)
                {
                    hand.GetChild(0).GetComponent<IInteractable>().Interact();
                }
                break;
            }
            case { } interactable:
                interactable.Interact();
                break;
            default:
                Debug.Log("Current interactable is not an interactable");
                break;
        }
    }
}
