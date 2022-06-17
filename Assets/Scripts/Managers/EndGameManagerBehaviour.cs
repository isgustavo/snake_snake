using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManagerBehaviour : MonoBehaviour
{
    public  int WinnerPlayerId { get; private set; }

    private void OnEnable()
    {
        ManagerLocator.RegisterManager(this);
    }

    private void Start()
    {
        if(ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnGameEnded += OnGameEnded;
        }
    }

    private void OnDisable()
    {
        if (ManagerLocator.TryGetManager(out IngameManagerBehaviour ingameManager))
        {
            ingameManager.OnGameEnded -= OnGameEnded;
        }

        ManagerLocator.UnRegisterManager<EndGameManagerBehaviour>();
    }

    public void OnGameEnded(int winnerPlayerId)
    {
        WinnerPlayerId = winnerPlayerId;

        SceneManager.LoadSceneAsync(SceneEnum.EndGameScene.ToString(), LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SceneEnum.EnvironmentScene.ToString());
    }

    public void BackToMainMenu ()
    {
        SceneManager.LoadSceneAsync(SceneEnum.MainMenuScene.ToString(), LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(SceneEnum.EndGameScene.ToString());
    }
}
