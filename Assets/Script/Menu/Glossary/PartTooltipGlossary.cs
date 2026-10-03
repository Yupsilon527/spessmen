using UnityEngine;
using UnityEngine.UI;

public class PartTooltipGlossary : PartCompBase
{
    private void Start()
    {
        Clear();
    }
    public override void ShowPart(PartScriptable part, bool showCombos)
    {
        base.ShowPart(part, showCombos);

        var boughtVariable = PlayerConfig.main.globalScope?.GetVariable("item_purchased_" + part.InternalName);
        var seenVariable = PlayerConfig.main.globalScope?.GetVariable("item_seen_" + part.InternalName);

        description.text += "<br><br>" + LanguageController.main.Translate("UI Table", "part_seen").Replace("%value%", seenVariable?.GetFloatValue().ToString("F0") ?? "0")
            +"<br>" + LanguageController.main.Translate("UI Table", "part_purchased").Replace("%value%", boughtVariable?.GetFloatValue().ToString("F0") ?? "0");
    }
}
