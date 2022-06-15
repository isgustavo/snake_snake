using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingController : MonoBehaviour
{
    [SerializeField]
    Image loadValueImage;

    int totalItems = 0;
    int currentItemsLoaded = 0;

    public void SetTotalItemsToLoad(int totalItems)
    {
        this.totalItems = totalItems;
        currentItemsLoaded = 0;
        loadValueImage.fillAmount = currentItemsLoaded;
    }

    public void OnLoadUpdate()
    {
        currentItemsLoaded += 1;
        loadValueImage.fillAmount = (float)currentItemsLoaded / (float)totalItems;
    }
}