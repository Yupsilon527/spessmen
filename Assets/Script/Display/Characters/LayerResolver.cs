using UnityEngine;

public class LayerResolver : Initializable
{
    public SpriteRenderer renderer;
    int sortingOrder = 0;
    public int sortingOrderDepth = 3;

    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref renderer);
    }
    public void ChangeOrder(int nOrder)
    {
        int delta = nOrder - sortingOrder;
        renderer.sortingOrder += delta * sortingOrderDepth;
        sortingOrder = nOrder;
    }
}
