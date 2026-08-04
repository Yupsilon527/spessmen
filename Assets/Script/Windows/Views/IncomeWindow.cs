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

            baseIncome.text = $"Race Complete: {bi:F1}g<br>Your Position: {pi:F1}g";

            if (interest > 0)
                bonusIncome.text = $"Interest: {interest:F1}g";
            else
                bonusIncome.text = "";


            float performance = DataItemPlayer.main.scope.GetVariable("gold_performance")?.GetFloatValue() ?? 0;

            if (performance > 0)
            {
                if (bonusIncome.text.Length > 0) bonusIncome.text += "<br>";
                bonusIncome.text += $"Distance ahead of Rival: {performance:F1}g";
            }
            else
                bonusIncome.text += "";

            totalIncome.text = $"Cash Out: {(bi+pi+interest+performance):F1}g";
            totalGold.text = $"Your cash: {DataItemPlayer.main.econ.gold.GetValue():F1}g";
        }
    }
}
