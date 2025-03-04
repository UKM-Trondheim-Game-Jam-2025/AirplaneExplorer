using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryComponent : MonoBehaviour
{
    [Header("Key Data")]
    [SerializeField] private List<KeyInteractable> heldKeys = new List<KeyInteractable>();

    public event Action<List<KeyInteractable>> OnInventoryUpdated;

    public List<KeyInteractable> GetKeys()
    {
        return heldKeys;
    }

    public void AddItem(KeyInteractable inItem)
    {
        if (heldKeys.Contains(inItem))
        {
            Debug.Log("already has this key");
            return;
        }
        heldKeys.Add(inItem);
        OnInventoryUpdated?.Invoke(heldKeys);
        Debug.Log("Added item succesfully: " + inItem);
    }

    public void RemoveItem(KeyInteractable inItem)
    {
        if (heldKeys.Contains(inItem))
        {
            heldKeys.Remove(inItem);
            OnInventoryUpdated?.Invoke(heldKeys);
        }
    }
}
