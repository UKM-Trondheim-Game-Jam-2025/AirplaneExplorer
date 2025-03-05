using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryComponent : MonoBehaviour
{
    [Header("Key Data")]
    [SerializeField] private List<KeyInteractable> heldKeys = new List<KeyInteractable>();
    [Header("Gun")]
    public GameObject gun;
    public bool hasGun;
    [SerializeField] private bool gunEquipped;

    public event Action<List<KeyInteractable>> OnInventoryUpdated;

    public List<KeyInteractable> GetKeys()
    {
        return heldKeys;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireGun();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleGun();
        }
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

    public void FireGun()
    {
        if (!gunEquipped) { return; }

        gun.GetComponent<Gun>().Fire();
    }

    public void ToggleGun()
    {
        Debug.Log("ToggledGun");
        gunEquipped = !gunEquipped;
        gun.SetActive(gunEquipped);
    }
}
