using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;


    }

    private void Start()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu ()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGameScene ()
    {
        SceneManager.LoadScene(2);
    }
}
