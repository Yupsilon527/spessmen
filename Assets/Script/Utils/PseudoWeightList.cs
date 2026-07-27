using System.Collections.Generic;
using UnityEngine;
public static class PseudoWeightList
{
    public static witem PickWeight<witem>(IEnumerable<witem> list, float chaos, float mult = 1 / 33) where witem : PseudoWeightEntry
    {
        if (list != null)
        {
            float total = Random.Range(0, GetTotal(list, chaos, mult));

            foreach (witem item in list)
            {
                if (item.Weight == 0 && item.chaosMultiplier == 0) continue;
                float w = item.GetWeight(chaos * mult);
                if (w > 0)
                {
                    total -= w;
                    if (total <= 0)
                        return item;
                }
            }
        }
        return null;
    }

    public static float GetTotal(IEnumerable<PseudoWeightEntry> value, float chaos, float mult = 1/33)
    {
        float total = 0;
        foreach (PseudoWeightEntry item in value)
        {
            float w = item.GetWeight(chaos * mult);
            if (w > 0) 
                total += w;
        }
        return total;
    }
}
[System.Serializable]
public class PseudoWeightEntry : WeightList.WeightEntry
{
    public float chaosMultiplier = 0;
    public PseudoWeightEntry(float weight, float chance) :base(weight)
    {
        Weight = Mathf.Max(1, weight);
        chaosMultiplier = chance;
    }
    public float GetWeight(float chaos)
    {
        return Weight  + chaos * chaosMultiplier;
    }
}
