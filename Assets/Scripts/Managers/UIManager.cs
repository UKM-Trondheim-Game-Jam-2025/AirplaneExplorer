using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform keyHUDField;
    [SerializeField] private GameObject keyUIPrefab;



    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one UI managers in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        GameManager.instance.playerInventory.OnInventoryUpdated += UpdateUI;
    }

    private void OnDestroy()
    {
        GameManager.instance.playerInventory.OnInventoryUpdated -= UpdateUI;
    }

    private void UpdateUI(List<KeyInteractable> heldKeys)
    {
        InitializeKeysUI(heldKeys);
    }

    private void InitializeKeysUI(List<KeyInteractable> heldKeys)
    {
        foreach (Transform child in keyHUDField)
        {
            Destroy(child.gameObject);
        }

        float xPos = 0f;

        for (int i = 0; i < heldKeys.Count; i++)
        {
            var key = heldKeys[i];
            GameObject keyUIElement = Instantiate(keyUIPrefab, keyHUDField);

            var image = keyUIElement.GetComponent<Image>();
            if (image != null)
            {
                var keyComponent = key;
                if (keyComponent != null)
                {
                    image.sprite = keyComponent.keySprite;
                }
            }
            RectTransform rectTransform = keyUIElement.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(xPos, 0);
            }
            xPos -= 50;
        }
    }
}