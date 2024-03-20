using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// SAVE AND LOAD SYSTEM TAKEN FROM "SHAPED BY RAIN STUDIOS" ON YOUTUBE: https://youtu.be/aUi9aijvpgs?si=zG_XG2aithbQpy7E https://youtu.be/ijVA5Z-Mbh8?si=4_8k6C5QAXMqG7HU
public class DataManager : MonoBehaviour
{

    List<IDataPersistance> dataPersistances = new List<IDataPersistance>();

    public GameData Data;

    public static DataManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.Log("found existing Data Manager instance, deleting new one");
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;


    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistances = GetAllDataObjects();
        LoadGame();
    }

    void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }
    private void Start()
    {
        
    }

    public void NewGame()
    {
        Data = new GameData();
        LoadGame();
    }

    public void SaveGame()
    {
        

        foreach(IDataPersistance obj in dataPersistances)
        {
            obj.SaveData(ref instance.Data);
        }

        //Debug.Log($"Data saved | Scene: {Data.CurrentScene} Spawn: {Data.SpawnPointName}");
    }

    public void LoadGame()
    {
        

        foreach (IDataPersistance obj in dataPersistances)
        {
            obj.LoadData(instance.Data);
        }

        Debug.Log($"Data Loaded | Scene: {Data.CurrentScene} Spawn: {Data.SpawnPointName}");
    }

    private List<IDataPersistance> GetAllDataObjects()
    {
        
        IEnumerable<IDataPersistance> objs = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        List<IDataPersistance> list = new List<IDataPersistance>(objs);

        Debug.Log($"Found {list.Count}");

        return list;
    }
}
