using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Greybox");
    
    }
    public void QuitGame()
    { 
    Application.Quit();
    
    }
}
