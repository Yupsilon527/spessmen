
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public bool autoSkip = false;
    public float minimumLoadingTime = 0f;

    public Action onLoadingFinished;
    public GameObject LoadingScreen;
    public GameObject LoadingCompleteScreen;

    public static LoadingScreenManager main;
    private void Awake()
    {
        main = this;
        SetLoaded(false);
    }
    public void SetLoaded(bool value)
    {
        if (LoadingScreen != null)
            LoadingScreen.gameObject.SetActive(!value);
        if (LoadingCompleteScreen != null)
            LoadingCompleteScreen.gameObject.SetActive(value);
        if (value && autoSkip)
            OnLoadingDisbanded();
    }
    public void OnLoadingDisbanded()
    {
        StartCoroutine(FadeToGameCoroutine());
    }
    IEnumerator FadeToGameCoroutine()
    {
        onLoadingFinished?.Invoke();
        yield return null;
        foreach (var gob in SceneManager.GetSceneByName(SceneTransitionManager.main.LoadingScene).GetRootGameObjects())
        {
            gob.SetActive(false);
        }
        SceneManager.UnloadSceneAsync(SceneTransitionManager.main.LoadingScene);
        gameObject.SetActive(false);
    }
}

