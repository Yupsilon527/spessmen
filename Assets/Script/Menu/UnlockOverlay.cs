using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UnlockOverlay : Window
{
    public TextMeshProUGUI unlockTitle;
    public Image unlockIcon;
    public float deltaRotation = 0;

    List<UnlockScriptable> queuedUnlocks = new();

    private void OnEnable()
    {
        UpdateUnlocks();
        HandleProceed();
    }
    void UpdateUnlocks()
    {
        queuedUnlocks.Clear();
        if (ResourceCache.main == null || ResourceCache.main.unlockables.Count == 0)
        {
            return;
        }
        foreach (var unlock in ResourceCache.main.unlockables)
        {
            if (!unlock.isUnlocked && unlock.unlockable.IsUnlocked())
            {
                queuedUnlocks.Add(unlock.unlockable);
                unlock.isUnlocked = true;
            }
        }
    }
    void ShowUnlock(UnlockScriptable unlock)
    {
        if (unlock is ShipScriptable ship)
        {
            if (unlockTitle != null)
                unlockTitle.text = LanguageController.main.Translate("Racers", unlock.InternalName);
            if (unlockIcon != null)
                unlockIcon.sprite = ship.portrait;
        }
        else if (unlock is PartScriptable part)
        {
            if (unlockTitle != null)
                unlockTitle.text = LanguageController.main.Translate("Parts", unlock.InternalName);
            if (unlockIcon != null)
                unlockIcon.sprite = part.icon;
        }
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, (UnityEngine.Random.value * 2 - 1) * deltaRotation);
    }
    public void HandleProceed()
    {
        if (queuedUnlocks.Count > 0)
        {
            ShowUnlock(queuedUnlocks[0]);
            queuedUnlocks.RemoveAt(0);
        }
        else
            Close();
    }
}
