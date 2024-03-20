using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// SAVE AND LOAD SYSTEM TAKEN FROM "SHAPED BY RAIN STUDIOS" ON YOUTUBE: https://youtu.be/aUi9aijvpgs?si=zG_XG2aithbQpy7E
public class DataManager : MonoBehaviour
{

    List<IDataPersistance> dataPersistances = new List<IDataPersistance>();

    public GameData Data;

    public static DataManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Error, found existing Data Manager instance");
        }
        instance = this;
    }

    private void Start()
    {
        dataPersistances = GetAllDataObjects();
        LoadGame();
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
    }

    public void LoadGame()
    {
        foreach (IDataPersistance obj in dataPersistances)
        {
            obj.LoadData(instance.Data);
        }
    }

    private List<IDataPersistance> GetAllDataObjects()
    {
        IEnumerable<IDataPersistance> objs = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(objs);
    }
}
