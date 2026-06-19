using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoComponent : MonoBehaviour
{
    public string message = "";
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out PlayerInfoOverlay player))
        {
            player.SelectObject(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out PlayerInfoOverlay player))
        {

            player.DeselectObject(this);
        }
    }
}
