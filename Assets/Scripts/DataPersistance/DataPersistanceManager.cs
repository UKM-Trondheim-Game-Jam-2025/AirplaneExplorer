using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool initializeDataIfNull = false;
    [Header("File Storage")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("DataPersistance manager already exists");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SaveGame();
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        //if (gameData.gameLost)
        //{
        //    NewGame();
        //    return;
        //}

        if (gameData == null)
        {
            Debug.Log("Initializing to default values");
            return;
        }

        foreach (var dataPersistantObj in dataPersistanceObjects)
        {
            dataPersistantObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("cannot save unitialized save data");
            return;
        }

        foreach (var dataPersistantObj in dataPersistanceObjects)
        {
            dataPersistantObj.SaveData(gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
