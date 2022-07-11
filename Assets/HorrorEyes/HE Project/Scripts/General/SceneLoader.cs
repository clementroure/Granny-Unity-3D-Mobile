using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    
    public bool isStartScene;
    [HideInInspector]
    public string sceneName;
    [Tooltip("Name of Main Menu scene")]
    public string mainMenuSceneName;
    public Image loadingBar;

    private void Awake()
    {
        if (!isStartScene)
        {
            sceneName = PlayerPrefs.GetString("GameLevel");
        }else
        {
            sceneName = mainMenuSceneName;
        }
    }

    private void Start()
    {
        StartCoroutine(SceneLoad());       
    }

    IEnumerator SceneLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progress = operation.progress / 0.9f;
            loadingBar.fillAmount = progress;
            yield return null;
        }

    }

}
