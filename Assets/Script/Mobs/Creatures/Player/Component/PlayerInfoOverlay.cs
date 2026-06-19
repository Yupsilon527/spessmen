using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoOverlay : PlayerComponent
{
    InfoComponent selectedObject;
    public GameObject tooltipContainer;
    public TextMeshProUGUI tooltipText;

    public void SelectObject(InfoComponent info)
    {
        selectedObject = info;
        DisplayText(info.message);
        parent.menu?.menuController?.MoveToTarget(parent.transform);
    }
    public void DeselectObject(InfoComponent info)
    {
        if (selectedObject == info) {
            selectedObject = null;
        ClearText();
    }
    }
    public void DisplayText(string text)
    {
        tooltipContainer.gameObject.SetActive(true);
        tooltipText.text = text;
    }
    public void ClearText()
    {
        tooltipContainer.gameObject.SetActive(false);
    }
}
