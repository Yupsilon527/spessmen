using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttachPoint : MonoBehaviour
{
    public string AttachPointName;
    private void Reset()
    {
        AttachPointName = gameObject.name.ToLower();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, .1f);
    }
}
