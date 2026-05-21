using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHarvestController : MonoBehaviour
{
    public GameObject ChunkPrefab;
    public static ResourceHarvestController active;
    int[] MaterialsHarvested;
    
    void Awake()
    {
        active = this;
        MaterialsHarvested = new int[(int)TerrainDefines.Element.total];
    }

    public void OnTileHarvested(Plixel tile)
    {
        if (WorldController.active.currentPhase
            == WorldController.GamePhase.Loading)
            return;

        TerrainDefines.Element tileElement = tile.GetElement();
        if (tileElement > TerrainDefines.Element.dirt)
        {
            if ((tileElement != TerrainDefines.Element.dirt && tileElement != TerrainDefines.Element.rock) || Random.value * 100 < TerrainDefines.MatterLossChance)
                MaterialsHarvested[(int)tileElement]++;
            if (MaterialsHarvested[(int)tileElement]>TerrainDefines.MatterInChunk)
            {
                MaterialsHarvested[(int)tileElement] -= TerrainDefines.MatterInChunk;
                TrySpawnChunk(ChunkPrefab, tile.parent.tiletoworldPosition(tile.position), tileElement, TerrainDefines.MatterInChunk);
            }
        }
    }
    public static GameObject TrySpawnChunk(GameObject prefab, Vector3 pos, TerrainDefines.Element element, int qty)
    {
        //TODO try fuse item, pool item
        GameObject orechunk = GameObject.Instantiate(prefab);
        orechunk.transform.position = pos;
        if (orechunk.TryGetComponent(out ChunkItem ore))
        {
            ore.ChangeElement(element, qty);
        }
        return orechunk;
    }
}
