using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : PartButtonBase
{
    public TextMeshProUGUI cooldownValue;
    public Image cooldownFill;
    public Button button;

    protected override void Redraw()
    {
        base.Redraw();
        
        cooldownFill.sprite = mPart.scriptable.icon;
        
        button.enabled = mPart?.correspondingAbility?.data?.function == ShipDefines.PartEvent.OnActivated;
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
        if (mPart.correspondingAbility!=null)
        {
            if (mPart.correspondingAbility.data.function == ShipDefines.PartEvent.OnRaceStart)
            {
                HideCooldown();
            }
            else
            {
                float cd = mPart.correspondingAbility.GetTimeRemaining();
                if (cd > 0) {
                    ShowCooldown(cd, mPart.correspondingAbility.GetDuration());
                }
                else
                {
                    HideCooldown();
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
}
