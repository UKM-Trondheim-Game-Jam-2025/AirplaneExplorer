using UnityEngine;

public class S_Door : MonoBehaviour, IInteractable
{
    private static readonly int Open = Animator.StringToHash("Open");

    [Header("Door References")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Renderer lockRender;
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool defaultOpen = false;
    [Header("Lock Settings")]
    [SerializeField] private int lockID;
    [SerializeField] private Color lockColor = Color.white;

    private void Start()
    {
        doorAnimator.SetBool(Open, defaultOpen);
        lockRender = GetComponent<Renderer>();
        if (lockRender != null)
        {
            lockRender.material.color = lockColor;
        }
    }

    public void Interact()
    {
        Debug.Log("Interacted with door");
        if (isLocked)
        {
            Debug.Log("The door is locked!");
            if (HasKey())
            {
                UnlockDoor();
            }
            else
            {
                return;
            }
        }

        doorAnimator.SetBool(Open, !doorAnimator.GetBool(Open));
    }

    private bool HasKey()
    {
        foreach (var key in GameManager.instance.playerInventory.GetKeys())
        {
            if (key != null && key.KeyID == lockID)
            {
                return true;
            }
        }
        return false;
    }

    private void UnlockDoor()
    {
        Debug.Log("Door unlocked!");
        isLocked = false;
        foreach (var key in GameManager.instance.playerInventory.GetKeys())
        {
            if (key != null && key.KeyID == lockID)
            {
                GameManager.instance.playerInventory.RemoveItem(key);
                break;
            }
        }
    }
}
