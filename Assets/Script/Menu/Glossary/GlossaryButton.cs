
public class GlossaryButton : PartCompBase
{
    public GlossaryView glossaryParent;
    public PartScriptable assignedObject;
    public void AssignItem(PartScriptable item)
    {
        assignedObject = item;
        ShowPart(item);
    }
    public override void Clear()
    {
        glossaryParent = null;
        base.Clear();
    }
    public  void OnPressed()
    {
        glossaryParent?.tooltip?.ShowPart(assignedObject);
    }
}
