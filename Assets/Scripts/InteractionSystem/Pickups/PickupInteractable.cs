using UnityEngine;

public class PickupInteractable : MonoBehaviour, IInteractable
{
    [Header("Pickup Settings")]
    [SerializeField] private bool usingPhysics = false;

    public void Interact()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (GameManager.instance.player.GetComponent<PlayerInteractionComponent>().hand != null)
        {
            if (GameManager.instance.player.GetComponent<PlayerInteractionComponent>().hand.childCount > 0)
            {
                Transform prevHeldItem = GameManager.instance.player.GetComponent<PlayerInteractionComponent>().hand.GetChild(0);
                if (usingPhysics)
                {
                    prevHeldItem.GetComponent<Rigidbody>().isKinematic = false;
                    prevHeldItem.GetComponent<Rigidbody>().useGravity = true;
                }
                prevHeldItem.parent = null;
            }
            if (usingPhysics && rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            transform.parent = GameManager.instance.player.GetComponent<PlayerInteractionComponent>().hand;
            transform.localPosition = Vector3.zero;
            Vector3 OffsetRotation = new Vector3(0, 90, 0);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + OffsetRotation);
        }
    }
}
