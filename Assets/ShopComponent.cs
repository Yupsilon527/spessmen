using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopComponent : MonoBehaviour
{
    [System.Serializable]
    public class ShopEntry
    {
        public GameObject Item;
        public float ItemCost;
    }
    public ShopEntry[] Shop = new ShopEntry[0];
}
