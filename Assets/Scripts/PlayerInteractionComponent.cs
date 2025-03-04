using UnityEngine;

public class PlayerInteractionComponent : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] public Transform hand;
    private Camera playerCamera;
    [Header("Interaction Data")]
    [SerializeField] float interactionDistance;
    private IInteractable currentInteractable;

    void Start()
    {
        playerCamera = transform.GetChild(2).GetComponent<Camera>();
    }

    void Update()
    {
        PerformRaycast();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteract();
        }
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

    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.green;
            Ray rayc = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Gizmos.DrawRay(rayc.origin, rayc.direction * interactionDistance);
        }
    }

    private void HandleInteract()
    {
        if (currentInteractable != null)
        {
            if (currentInteractable is IInteractable interactable)
            {
                interactable.Interact();
            }
            else
            {
                Debug.Log("Current interactable is not an interactable");
            }
        }
    }
}
