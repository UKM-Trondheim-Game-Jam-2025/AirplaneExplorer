using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour, IInteractable
{
    [Header("Gun Settings")]
    private Camera playerCamera;
    [SerializeField] private float fireRange;
    [SerializeField] private Transform muzzlePivot;
    [SerializeField] private GameObject hitEffect, muzzleEffect;
    [SerializeField] public AudioClip equipSound, fireClip;
    [SerializeField] private InventoryComponent owner;
    [SerializeField] public int bullets = 3;
    public Sprite gunSprite;
    [Header("Debugging Logic")]
    [SerializeField] bool firing = false;

    private void Start()
    {
        if (GameManager.instance.playerInventory != null)
        {
            owner = GameManager.instance.playerInventory;
        }
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }
    public void Interact()
    {
        Debug.Log("Called interact on: " + this.gameObject.name);
        if (!owner) return;

        owner.hasGun = true;
        owner.gun = this.gameObject;
        owner.gunUI.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = gunSprite;
        owner.InitializeGunUI(bullets);
        owner.gunUI.SetActive(true);
        this.transform.parent = GameManager.instance.player.GetComponent<PlayerInteractionComponent>().hand;
        this.transform.position = transform.parent.position + new Vector3(0f, -.5f, 0f);
        this.transform.localRotation = Quaternion.Euler(transform.forward);
        owner.ToggleGun();
    }

    public void Fire()
    {
        if (bullets > 0)
        {
            Debug.Log("fired gun: " + owner.name + " - " + this.gameObject.name);
            firing = true;
            RaycastHit hit;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out hit, fireRange))
            {
                owner.PlaySound(fireClip);
                Instantiate(hitEffect, hit.point, Quaternion.Euler(hit.normal));
                Instantiate(muzzleEffect, muzzlePivot.position, Quaternion.Euler(Vector3.forward));
                bullets--;
                owner.UpdateBulletUI();
                GameObject hitObject = hit.collider.gameObject;
                switch (hitObject.tag)
                {
                    case "Lock":
                        hitObject.transform.parent.GetChild(0).transform.GetChild(0).gameObject.GetComponent<S_Door>().OverrideLock();
                        Debug.Log("Hit: " + hitObject.name);
                        break;
                    case "Window":
                        Debug.Log("you lose");
                        GameManager.instance.hasLost = true;
                        GameManager.instance.SwitchSceneToLoseScene();
                        break;
                    default:
                        break;
                }
                firing = false;
            }
        }
        else
        {
           DestroyGun();
        }
    }

    void DestroyGun()
    {
        owner.hasGun = false;
        owner.ToggleGun();
        Destroy(this.gameObject);
        owner.gunUI.gameObject.SetActive(false);
    }
}