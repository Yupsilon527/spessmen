using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityPreview : ShipPreview
{
    public HashSet<AbilityButton> buttons = new();
    public void LoadPlayerShip(DataItemShip ship)
    {
        if (ship == null) return;
        foreach (var slot in ship.parts)
        {
            GenerateButton(slot);
        }
    }
    public void GenerateButton(DataItemPart part)
    {
        GameObject parent = TokenPool.PoolItem(TokenPrefab);
        AbilityButton token = parent.GetComponent<AbilityButton>();
        token.transform.localScale = Vector3.one * transform.localScale.x;
        token.FromPart(part, true);
        buttons.Add(token);

        token.SnapToGrid(GetComponent<RectTransform>());
        token.AdjustRotation(part.rotation);
    }
    public override void Clear()
    {
        foreach (var token in buttons)
            TokenPool.DeactivateObject(token.gameObject);
    }
}
