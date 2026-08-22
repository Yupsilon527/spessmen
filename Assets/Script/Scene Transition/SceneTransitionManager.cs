
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionManager : Initializable
{
    public bool RequireGameEssentials = false;
    public string LoadingScene;
    public string GameEssentials;

    public static SceneTransitionManager main;
    protected override void Initialize()
    {
        base.Initialize();
        if (main == null)
        {
            main = this;
            GameObject.DontDestroyOnLoad(gameObject);
            if (RequireGameEssentials)
                StartCoroutine(LoadSceneAsync(GameEssentials, LoadSceneMode.Additive));
        }
        else
        {
            GameObject.DestroyImmediate(this);
        }
    }
    public void TransitionScene(string SceneName)
    {
        StartCoroutine(TransitionSceneCoroutine(SceneName));
    }
    bool SceneExistsInBuild(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
    public IEnumerator TransitionSceneCoroutine(string SceneName)
    {
        yield return LoadSceneAsync(LoadingScene);
        yield return new WaitForSecondsRealtime(.5f);
        yield return LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(LoadingScene);
    }
    public void TransitionGameScene(string SceneName, Action sceneCompletedAction)
    {
        StartCoroutine(TransitionGameSceneCoroutine(SceneName, sceneCompletedAction));
    }
    public IEnumerator TransitionGameSceneCoroutine(string SceneName, Action sceneCompletedAction)
    {
        yield return LoadSceneAsync(LoadingScene);

        float minLoadingTime = Time.time;
        if (LoadingScreenManager.main != null)
        {
            minLoadingTime += LoadingScreenManager.main.minimumLoadingTime / Time.timeScale;
            LoadingScreenManager.main.onLoadingFinished = sceneCompletedAction;
            yield return null;
        }
        if (SceneExistsInBuild(GameEssentials))
            yield return LoadSceneAsync(GameEssentials, LoadSceneMode.Additive);
        else
            Debug.LogWarning("Invalid scene " + GameEssentials);
        if (SceneExistsInBuild(SceneName))
            yield return LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        else
            Debug.LogError("Invalid scene " + SceneName);

        while (minLoadingTime > Time.time)
            yield return null;

        yield return new WaitForEndOfFrame();
        LoadingScreenManager.main?.SetLoaded(true);
    }
    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
    IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode sceneMode)
    {
        for (int iS = 0; iS < SceneManager.sceneCount; iS++)
            if (SceneManager.GetSceneAt(iS).name == sceneName)
                yield return null;
        yield return SceneManager.LoadSceneAsync(sceneName, sceneMode);
    }
}

