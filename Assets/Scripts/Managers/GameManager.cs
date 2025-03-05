using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Player")]
    public InventoryComponent playerInventory;
    public GameObject player;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        instance = this;

        player = GameObject.Find("Player");
        playerInventory = player.GetComponent<InventoryComponent>();
    }

    private void Start()
    {
        
    }
}