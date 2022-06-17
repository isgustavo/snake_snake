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
    readonly List<SceneEnum> initialScenes = new List<SceneEnum>()
    {
        //SceneEnum.EnvironmentScene
    };

    private bool isGameReady;
    public bool IsGameReady
    {
        get
        {
            return isGameReady;
        }
        set
        {
            isGameReady = value;
            if(isGameReady)
            {
                OnGameIsReady?.Invoke();
            }
        }
    }
    public bool IsObjectPoolFinished { get; private set; }

    public event Action OnGameIsReady;

    protected void OnEnable()
    {
        IsGameReady = false;
        IsObjectPoolFinished = false;
        ManagerLocator.RegisterManager(this);
    }

    private IEnumerator Start()
    {
        AsyncOperation loadSceneAsync = null;

        loadSceneAsync = SceneManager.LoadSceneAsync(SceneEnum.LoadScene.ToString(), LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadSceneAsync.isDone == true);

        UILoadingController loadingController = FindObjectOfType<UILoadingController>();

        CreatePool(loadingController);

       yield return StartCoroutine(LoadScenesCoroutine(loadSceneAsync, loadingController));

        yield return new WaitUntil(() => IsObjectPoolFinished);

        SceneManager.UnloadSceneAsync(SceneEnum.LoadScene.ToString());
        
        IsGameReady = true;
    }

    private void OnDisable()
    {
        ManagerLocator.UnRegisterManager<InitializeManagerBehaviour>();
    }

    IEnumerator LoadScenesCoroutine(AsyncOperation loadSceneAsync, UILoadingController loadingController)
    {
        foreach (SceneEnum scene in initialScenes)
        {
            loadSceneAsync = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);

            yield return new WaitUntil(() => loadSceneAsync.isDone == true);
            loadingController.OnLoadUpdate();
        }

        yield return 0;
    }

    private void CreatePool(UILoadingController loadingController)
    {
        if (ManagerLocator.TryGetManager(out PoolManagerBehaviour poolManager))
        {
            poolManager.OnPoolFinished += OnObjectPoolFinished;
            poolManager.OnPoolLoadUpdate += loadingController.OnLoadUpdate;

            StartCoroutine(poolManager.CreatePool());

            loadingController.SetTotalItemsToLoad(initialScenes.Count + poolManager.InitialPoolCount);
        }
        else
        {
            IsObjectPoolFinished = true;
        }
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


