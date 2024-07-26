using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public Slider loadingSlider;

    private void Update()
    {
        if (WorldController.active != null)
        {
            bool isLoading = WorldController.active.currentPhase == WorldController.GamePhase.Loading;
            SetEnabled(isLoading);

            if (isLoading)
            {
                float progress = CalculateLoadingProgress();
                UpdateLoadingSlider(progress);
            }
        }
        else
        {
            SetEnabled(false);
        }
    }

    void SetEnabled(bool nValue)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(nValue);
        }
    }

    float CalculateLoadingProgress()
    {
        // Can edit here. Add custom logic.
        // It only cuts half.
        return 0.5f;
    }

    void UpdateLoadingSlider(float progress)
    {
        if (loadingSlider != null)
        {
            loadingSlider.value = progress;
        }
    }
}
