using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public string sceneName;

    [ContextMenu("Change Scene")]
    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
