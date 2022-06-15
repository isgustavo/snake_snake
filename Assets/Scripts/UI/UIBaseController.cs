using System.Collections.Generic;
using UnityEngine;

public abstract class UIBaseController : MonoBehaviour, IControllableUI
{
    //[SerializeField]
    //protected List<IControllableUI> listOfNavigation;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public abstract void Resume();
    public abstract void Updater();
    public abstract void Background();
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    //public void NavigateTo<T>(object value) where T : IControllableUI
    //{
        /*foreach (IControllableUI controllableUI in listOfNavigation)
        {
            if (controllableUI is T)
            {
                if(ManagerLocator.TryGetManager<UIManagerBehaviour>(out UIManagerBehaviour uiManager))
                {
                    uiManager.Show(controllableUI, value);
                }
            }
        }*/
    //}
}
