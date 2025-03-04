using UnityEngine;

public class S_Door : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");

    [Header("Door References")]
    [SerializeField] private Animator doorAnimator;
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool defaultOpen = false;

    private void Start()
    {
        doorAnimator.SetBool(Open, defaultOpen);
    }
    
    private void Interact()
    {
        if (isLocked)
        {
            Debug.Log("The door is locked!");
            return;
        }
        
        doorAnimator.SetBool(Open, !doorAnimator.GetBool(Open));
    }
}
