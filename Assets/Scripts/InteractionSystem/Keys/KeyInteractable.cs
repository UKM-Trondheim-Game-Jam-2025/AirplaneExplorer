using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable, IDataPersistance
{
    [Header("Key Data")]
    public int KeyID;
    public Sprite keySprite;
    public Color keyColor = Color.white;
    public bool collected;

    public void Interact()
    {
        if (!collected)
        {
            Debug.Log("collected key");
            GameManager.instance.playerInventory.AddItem(this);
            collected = true;
            this.gameObject.SetActive(false);
        }
    }

    public void LoadData(GameData data)
    {/*
        data.keysCollected.TryGetValue(KeyID, out var keyData);
        collected = keyData.collected;*/
        if (data.keysCollected.TryGetValue(KeyID, out bool isCollected) && isCollected)
        {
            collected = isCollected;
            GameManager.instance.playerInventory.AddItem(this);
            this.gameObject.SetActive(false);
        }
        UIManager.instance.UpdateUI(GameManager.instance.playerInventory.GetKeys());
    }

    public void SaveData(GameData data) 
    {
        if (data.keysCollected.ContainsKey(KeyID))
        {
            data.keysCollected[KeyID] = collected;
        }
        else
        {
            data.keysCollected.Add(KeyID, collected);
        }
    }
}