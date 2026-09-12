using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlossaryView : MonoBehaviour
{
    public ContainerMenu list;
    public PartTooltip tooltip;

    public List<PartScriptable> parts=new();
    public List<PartScriptable> filtered=new();
    public List<GlossaryButton> buttons=new();

    public void FilterParts()
    {
        parts = filtered.Where(p => p.IsUnlocked()).ToList();
    }
    public void PopulateList()
    {
        foreach (var item in filtered)
        {
            var gObject = list.PoolEmptyContainer();
            if (gObject.TryGetComponent(out GlossaryButton gloBtn))
                {
                gloBtn.glossaryParent = this;
                gloBtn.AssignItem(item);
                buttons.Add(gloBtn);
            }
        }
        list.Sort(sortListByName);
    }
    public static Action<List<GameObject>> sortListByName = (list) =>
    {
        list.Sort((a, b) =>
        {
            if (a.TryGetComponent(out GlossaryButton tA) && b.TryGetComponent(out GlossaryButton tB))
            {
                return tA.title.text.CompareTo(tB.title.text);
            }
            else return 0;
        });
    };
}
