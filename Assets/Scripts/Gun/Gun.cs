using UnityEngine;

public class Gun : MonoBehaviour, IInteractable
{
    [Header("Gun Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] float fireRange;
    [SerializeField] Transform muzzlePivot;
    [SerializeField] GameObject hitEffect, muzzleEffect;
    [SerializeField] InventoryComponent owner;
    [SerializeField] int bullets = 3;
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
                Instantiate(hitEffect, hit.point, Quaternion.identity);
                Instantiate(muzzleEffect, muzzlePivot.position, Quaternion.identity);
                bullets--;
                GameObject hitObject = hit.collider.gameObject;
                switch (hitObject.tag)
                {
                    case "Lock":
                        hitObject.transform.parent.GetChild(0).transform.GetChild(0).gameObject.GetComponent<S_Door>().OverrideLock();
                        Debug.Log("Hit: " + hitObject.name);
                        break;
                    case "Window":
                        // lose/end game
                        break;
                    case "":
                        break;
                    default:
                        break;
                }
                firing = false;
            }
        }
        else
        {
            owner.hasGun = false;
            Destroy(this.gameObject);
        }
    }
}