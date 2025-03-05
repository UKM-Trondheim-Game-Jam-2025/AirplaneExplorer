using UnityEngine;

public class S_Door : MonoBehaviour, IInteractable
{
    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int IsLocked = Animator.StringToHash("IsLocked");

    [Header("Door References")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Renderer lockRender;
    [SerializeField] private GameObject lockObject;
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool defaultOpen = false;
    [Header("Lock Settings")]
    [SerializeField] private int lockID;
    [SerializeField] private Color lockColor = Color.white;

    private void Start()
    {
        doorAnimator.SetBool(IsLocked, isLocked);
        doorAnimator.SetBool(Open, defaultOpen);
        if (lockRender == null)
        {
            lockRender = lockObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>();
        }
        if (lockRender != null)
        {
            lockRender.material.color = lockColor;
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log("The door is locked!");
            if (HasKey())
            {
                UnlockDoor();
                doorAnimator.SetBool(IsLocked, false);
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

    public void OverrideLock()
    {
        isLocked = false;
        doorAnimator.SetBool(IsLocked, isLocked);
        doorAnimator.SetBool(Open, !doorAnimator.GetBool(Open));
    }
}
