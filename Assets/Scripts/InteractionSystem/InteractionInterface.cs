using UnityEngine;

public interface IInteractable
{
    void Interact()
    {
    }

    GameObject gameObject { get; }
}