using TMPro;
using UnityEngine;

public class IncomeWindow : MonoBehaviour
{
    public TextMeshProUGUI baseIncome, bonusIncome, totalIncome, totalGold;

    private void OnEnable()
    {
        UpdateValues();
    }
    void UpdateValues()
    {
        if (DataItemPlayer.main != null)
        {
            float bi = DataItemPlayer.main.scope.GetVariable("gold_race")?.GetFloatValue() ?? 0;
            float pi = DataItemPlayer.main.scope.GetVariable("gold_position")?.GetFloatValue() ?? 0;

            float interest = DataItemPlayer.main.scope.GetVariable("gold_interest")?.GetFloatValue() ?? 0;

            string completionLabel = LanguageController.main.Translate("Leaderboard","raceIncome").Replace("%value%", bi.ToString("F1"));
            string positionLabel = LanguageController.main.Translate("Leaderboard", "performanceIncome").Replace("%value%", pi.ToString("F1"));

            baseIncome.text = $"{completionLabel}<br>{positionLabel}";

            if (interest > 0)
                bonusIncome.text = LanguageController.main.Translate("Leaderboard", "interestIncome").Replace("%value%", interest.ToString("F1"));
            else
                bonusIncome.text = "";


            float performance = DataItemPlayer.main.scope.GetVariable("gold_performance")?.GetFloatValue() ?? 0;

            if (performance > 0)
            {
                if (bonusIncome.text.Length > 0) bonusIncome.text += "<br>";
                bonusIncome.text += LanguageController.main.Translate("Leaderboard", "rivalIncome").Replace("%value%", performance.ToString("F1")); ;
            }
            else
                bonusIncome.text += "";

            totalIncome.text = LanguageController.main.Translate("Leaderboard", "cashOutTotal").Replace("%value%", (bi + pi + interest + performance).ToString("F1")); ;
            totalGold.text = LanguageController.main.Translate("UI Table", "Gold Label").Replace("%gold%", DataItemPlayer.main.econ.gold.GetValue().ToString("F0")); ;
        }
    }
}
