using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Player")]
    public InventoryComponent playerInventory;
    public GameObject player;

    [Header("Lose condition")]
    public bool hasLost;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        instance = this;

        //player = GameObject.Find("Player");
        playerInventory = player.GetComponent<InventoryComponent>();
    }

    public void SwitchSceneToLoseScene()
    {
        SceneManager.LoadScene("3_EndCutscene");
    }

    public void RestartGame()
    {
        //DataPersistanceManager.Instance.NewGame();
        SceneManager.LoadScene("0_MainMenu");
    }
}