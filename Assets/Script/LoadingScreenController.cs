using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    private void Update()
    {
        if (WorldController.active != null)
            SetEnabled(WorldController.active.currentPhase == WorldController.GamePhase.Loading);
        else
            SetEnabled(false);
    }
    void SetEnabled(bool nValue)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(nValue);
        }
    }
}
