using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : NetworkBehaviour
{
    public static LoadScene Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LoadScene();
            }
            
            return _instance;
        }
    }
    private static LoadScene _instance;

    public void LoadNextScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Stage1", LoadSceneMode.Single);
        SceneManager.LoadSceneAsync("Stage1", LoadSceneMode.Single);
    }
}