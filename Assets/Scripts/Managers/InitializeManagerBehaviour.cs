using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneEnum
{
    MainScene,
    LoadScene,
    EnvironmentScene,
    MainMenuScene,
    EndGameScene
}

public class InitializeManagerBehaviour : MonoBehaviour
{
    readonly List<SceneEnum> initialScenes = new()
    {
        //SceneEnum.EnvironmentScene
    };

    public bool IsGameReady { get; private set; }
    public bool IsObjectPoolFinished { get; private set; }

    public event Action OnGameIsReady;

    protected void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
        IsGameReady = false;
        IsObjectPoolFinished = false;
    }

    private IEnumerator Start()
    {
        AsyncOperation loadSceneAsync = null;

        loadSceneAsync = SceneManager.LoadSceneAsync(SceneEnum.LoadScene.ToString(), LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadSceneAsync.isDone == true);

        UILoadingController loadingController = FindObjectOfType<UILoadingController>();

        if (ManagerLocator.TryGetManager<PoolManagerBehaviour>(out PoolManagerBehaviour poolManager))
        {
            poolManager.OnPoolFinished += OnObjectPoolFinished;
            poolManager.OnPoolLoadUpdate += loadingController.OnLoadUpdate;

            StartCoroutine(poolManager.CreatePool());
        }
        else
        {
            IsObjectPoolFinished = true;
        }

        loadingController.SetTotalItemsToLoad(initialScenes.Count + poolManager.InitialPoolCount);

        yield return new WaitForEndOfFrame();

        foreach (SceneEnum scene in initialScenes)
        {
            loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

            yield return new WaitUntil(() => loadSceneAsync.isDone == true);
            loadingController.OnLoadUpdate();
        }

        yield return new WaitUntil(() => IsObjectPoolFinished);

        SceneManager.UnloadSceneAsync(SceneEnum.LoadScene.ToString());
        IsGameReady = true;
        OnGameIsReady?.Invoke();
    }

    private void OnObjectPoolFinished()
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.OnPoolFinished -= OnObjectPoolFinished;
        }

        IsObjectPoolFinished = true;
    }
}


