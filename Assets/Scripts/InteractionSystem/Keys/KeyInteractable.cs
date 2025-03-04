using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{
    [Header("Key Data")]
    public int KeyID;
    public Sprite keySprite;
    public Color keyColor = Color.white;

    public void Interact()
    {
        Debug.Log("collected key");
        GameManager.instance.playerInventory.AddItem(this);
        this.gameObject.SetActive(false);
    }
}