using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Diagnostics.Tracing;
using UnityEngine.InputSystem.Utilities;
using System;
using UnityEngine.ProBuilder;

// SAVE AND LOAD SYSTEM TAKEN FROM "SHAPED BY RAIN STUDIOS" ON YOUTUBE: https://youtu.be/aUi9aijvpgs?si=zG_XG2aithbQpy7E https://youtu.be/ijVA5Z-Mbh8?si=4_8k6C5QAXMqG7HU
public class DataManager : MonoBehaviour
{

    List<IDataPersistance> dataPersistances = new List<IDataPersistance>();

    public GameData Data;

    private IDisposable ButtonListener;

    GameOverScreen gameOverData;

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
        GetAllPickups();
        LoadGame();
    }

    void OnSceneUnloaded(Scene scene)
    {
        instance.Data.PickupsInScene = new Dictionary<GameObject, Vector3>();
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
        GetAllPickups();

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

        foreach(GameObject obj in Data.PickupsInScene.Keys)
        {
            

            
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

    // gets all pickups in the scene and stores them in the dictionary with their starting location
    private void GetAllPickups()
    {
        instance.Data.PickupsInScene = new Dictionary<GameObject, Vector3>();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Pickup");

        foreach(GameObject obj in objs)
        {
            instance.Data.PickupsInScene.Add(obj, obj.transform.position);
        }
    }

    public IEnumerator FadeLoad(float fadeTime, float waitTime)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        ScreenTransition fade = GameObject.FindAnyObjectByType<ScreenTransition>();

        // restrict the player movement
        player.currentRestriction = PlayerController.MovementRestrictions.noMovement;

        // fade to black
        if(fade != null)
        {
            fade.Out();
        }
        

        yield return new WaitForSeconds(fadeTime);

        // Load the game
        LoadGame();

        yield return new WaitForSeconds(waitTime);

        // fade back in
        if (fade != null)
        {
            fade.In();
        }

        // give player movement
        player.currentRestriction = PlayerController.MovementRestrictions.None;

    }

    public IEnumerator HintLoad(float waitTime, GameOverScreen hintData)
    {
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        ScreenTransition fade = GameObject.FindAnyObjectByType<ScreenTransition>();

        gameOverData = hintData;

        // restrict the player movement
        player.currentRestriction = PlayerController.MovementRestrictions.noMovement;

        // fade to black
        if (fade != null)
        {
            //fade.Out();
        }
        

        

        hintData.PlayHint();

        ButtonListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);

        

        yield return new WaitForSeconds(waitTime);

        // Load the game
        LoadGame();




    }

    void OnAnyButtonPressed(InputControl button)
    {
        

        ButtonListener.Dispose();

        gameOverData.StopHint();


        // give player movement
        GameObject.FindObjectOfType<PlayerController>().currentRestriction = PlayerController.MovementRestrictions.None;
    }
}
