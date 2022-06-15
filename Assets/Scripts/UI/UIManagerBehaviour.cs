using System.Collections.Generic;
using UnityEngine;
public interface IControllableUI
{
    public void Show();
    public void Resume();
    public void Updater();
    public void Background();
    public void Hide();
}

public class UIManagerBehaviour : MonoBehaviour
{
    private Stack<IControllableUI> controllableUIStack = new();

    private void OnEnable()
    {
        ManagerLocator.RegisterManager<UIManagerBehaviour>(this);
    }

    private void Update()
    {
        if(controllableUIStack.TryPeek(out IControllableUI ui))
        {
            ui.Updater();
        }
    }

    public void AddUI(IControllableUI ui)
    {
        if (controllableUIStack.TryPeek(out IControllableUI topUI))
        {
            topUI.Background();
        }

        controllableUIStack.Push(ui);
        ui.Show();
    }

    public void RemoveUI()
    {
        IControllableUI l = controllableUIStack.Pop();
        l.Hide();

        if (controllableUIStack.TryPeek(out IControllableUI topUI))
        {
            topUI.Resume();
        }
    }
}
