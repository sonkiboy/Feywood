using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("PreliminaryGraybox");
    
    }
    public void QuitGame()
    { 
    Application.Quit();
        Debug.Log("QUIT GAME");
    
    }
}
