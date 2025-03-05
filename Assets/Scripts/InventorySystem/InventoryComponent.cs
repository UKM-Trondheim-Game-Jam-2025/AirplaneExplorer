using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryComponent : MonoBehaviour
{
    [Header("Key Data")]
    [SerializeField] private List<KeyInteractable> heldKeys = new List<KeyInteractable>();
    [Header("Gun")]
    public GameObject gun;
    public bool hasGun;
    [SerializeField] private bool gunEquipped;
    public GameObject gunUI, bulletUI;
    public AudioSource inventoryAudio;

    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 shrunkScale = new Vector3(0.7f, 0.7f, 1f);
    private float normalAlpha = 1f;
    private float shrunkAlpha = 0.5f;

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
        inventoryAudio.PlayOneShot(gun.GetComponent<Gun>().equipSound);
        gunEquipped = !gunEquipped;
        gun.SetActive(gunEquipped);
        ToggleGunUIState(gunEquipped);
    }

    public void PlaySound(AudioClip soundToPlay)
    {
        inventoryAudio.PlayOneShot(soundToPlay);
    }

    private void ToggleGunUIState(bool isShrinking)
    {
        if (!isShrinking)
        {
            StartCoroutine(ResizeGunUI(shrunkScale, shrunkAlpha));
        }
        else
        {
            StartCoroutine(ResizeGunUI(normalScale, normalAlpha));
        }
    }

    public void InitializeGunUI(int bulletCount)
    {
        foreach (Transform child in gunUI.transform.GetChild(0).transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bulletUIElement = Instantiate(bulletUI, gunUI.transform.GetChild(0).transform);
            RectTransform rectTransform = bulletUIElement.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * -20, -60);
        }
    }

    public void UpdateBulletUI()
    {
        int i = 0;
        foreach (Transform child in gunUI.transform.GetChild(0).transform)
        {
            child.gameObject.SetActive(i < this.gun.GetComponent<Gun>().bullets);
            i++;
        }
    }

    private IEnumerator ResizeGunUI(Vector3 targetScale, float targetAlpha)
    {
        float time = 0f;
        Vector3 initialScale = gunUI.transform.localScale;
        Image gunImage = gunUI.transform.GetChild(0).gameObject.GetComponent<Image>();
        float initialAlpha = gunImage.color.a;

        while (time < .8f)
        {
            time += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(time);
            gunUI.transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpFactor);
            Color currentColor = gunImage.color;
            currentColor.a = Mathf.Lerp(initialAlpha, targetAlpha, lerpFactor);
            gunImage.color = currentColor;

            yield return null;
        }
    }
}