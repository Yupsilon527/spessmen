using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : PartButtonBase, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI cooldownValue;
    public Image cooldownFill;
    public Button button;

    protected override void Redraw()
    {
        base.Redraw();
        
        cooldownFill.sprite = mPart.scriptable.icon;
        
        button.enabled = mPart?.correspondingAbility?.data?.function == ShipDefines.PartEvent.OnActivated;
        HideCooldown();
    }
    public override void AdjustRotation(int rotation)
    {
        base.AdjustRotation(rotation);
        cooldownFill.fillOrigin = rotation;

        cooldownValue.transform.rotation = Quaternion.identity;
    }
    private void Update()
    {
        UpdateCooldown();
    }
    void UpdateCooldown()
    {
        HideCooldown();
        if (mPart.correspondingAbility!=null)
        {
            if (mPart.correspondingAbility.data.function != ShipDefines.PartEvent.OnRaceStart)
            {
                float cd = mPart.correspondingAbility.GetTimeRemaining();
                if (cd > 0) {
                    ShowCooldown(cd, mPart.correspondingAbility.GetDuration());
                }
            }
            button.interactable = mPart?.correspondingAbility?.CanBeActivated(TourneyController.main.GetPlayerRacer()) ?? false;
        }
    }
    void ShowCooldown(float cd, float total)
    {
        cooldownValue.text = $"{Mathf.Round(cd*10)/10}";
        cooldownFill.fillAmount = cd/total;
    }
    void HideCooldown()
    {
        cooldownValue.text = "";
        cooldownFill.fillAmount = 0;
    }
    public void OnClick()
    {
            mPart?.correspondingAbility?.Activate(TourneyController.main.GetPlayerRacer(), ShipDefines.PartEvent.OnActivated);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mPart != null && ViewManager.Instance.race != null && ViewManager.Instance.race.tooltip != null)
        {
            ViewManager.Instance.race.tooltip.ShowPart(mPart.scriptable);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ViewManager.Instance.race != null && ViewManager.Instance.race.tooltip != null)
        {
            ViewManager.Instance.race.tooltip.Clear();
        }
    }
}
