using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] SceneReference EndCutscene;
    
    [Header("Pickup Settings")]
    [SerializeField] private bool usingPhysics = false;
    [SerializeField] private bool isTheLuggage = false;
    private PlayerInteractionComponent _playerInteractionComponent;
    private Rigidbody _rb;
    public Color mainColor;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInteractionComponent = GameManager.instance.player.GetComponent<PlayerInteractionComponent>();
        _rb.useGravity = usingPhysics;
        _rb.isKinematic = !usingPhysics;
        Renderer rend = GetComponent<Renderer>();
        mainColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        rend.material.color = mainColor;
    }

    public void Interact()
    {
        if (!_playerInteractionComponent.hand) return;
        if (_playerInteractionComponent.hand == transform.parent)
        {
            Drop();
        }
        else
        {
            PickUp();
        }
        
    }

    private void PickUp()
    {
        if (!isTheLuggage)
        {
            if (_playerInteractionComponent.hand.childCount > 0)
            {
                Transform prevHeldItem = _playerInteractionComponent.hand.GetChild(0);
                if (usingPhysics)
                {
                    prevHeldItem.GetComponent<Rigidbody>().isKinematic = false;
                    prevHeldItem.GetComponent<Rigidbody>().useGravity = true;
                }
                prevHeldItem.parent = null;
            }
            if (usingPhysics && _rb)
            {
                _rb.useGravity = false;
                _rb.isKinematic = true;
            }
            transform.parent = _playerInteractionComponent.hand;
            transform.localPosition = Vector3.zero;
            Vector3 offsetRotation = new Vector3(0, 90, 0);
            transform.localRotation = Quaternion.Euler(transform.forward + offsetRotation);
        }
        else
        {
            WinTheGame();
        }
    }

    private void Drop()
    {
        transform.parent = null;
        if (!usingPhysics || !_rb) return;
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.AddForce(-transform.right * 5f, ForceMode.Impulse);
    }

    private void WinTheGame()
    {
        Debug.Log("Won the game");
        // switch Scene or simply play winning cutscene
        SceneManager.LoadScene(EndCutscene.Name);
    }
}
