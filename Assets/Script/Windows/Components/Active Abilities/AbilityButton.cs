using System.Linq;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : PartButtonBase, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI cooldownValue;
    public Image cooldownFill;
    public Button button;
    Ability[] corresponding = new Ability[0];
    public override void FromPart(DataItemPart part, bool draw)
    {
        base.FromPart(part, false);
        corresponding = GetCorrespondingAbilities();
        if (draw)
            Redraw();
    }
    public Ability[] GetCorrespondingAbilities()
    {
        return TourneyController.main.GetPlayerRacer().abilities.GetAbilitiesCorrespondingToPart(mPart.scriptable);
    }
    protected override void Redraw()
    {
        base.Redraw();
        
        cooldownFill.sprite = mPart.scriptable.icon;
        
        button.enabled = corresponding.Any(a => a.data.function == ShipDefines.PartEvent.OnActivated);
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
        float longestCD = 0;
        float longestExhaust = 0;
        foreach (var ability in corresponding)
        {
            if (ability.data.function != ShipDefines.PartEvent.OnRaceStart)
            {
                float cd = ability.GetTimeRemaining();
                if (cd > longestCD)
                {
                    longestCD = cd;
                }
            }
        }
            button.interactable = corresponding.Any(a=>a.CanBeActivated());
        if (longestCD>0)
            ShowCooldown(longestCD, longestExhaust);
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
        if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.racing)
        foreach (var ability in corresponding)
            ability.Activate( ShipDefines.PartEvent.OnActivated);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mPart != null && ViewManager.Instance.race != null && ViewManager.Instance.race.tooltip != null)
        {
            ViewManager.Instance.race.tooltip.ShowPart(mPart.scriptable,false);
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
