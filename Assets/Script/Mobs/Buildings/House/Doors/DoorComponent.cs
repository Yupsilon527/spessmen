using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    Animator animator;
    CompartimentComponent House;
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (House == null)
            House = GetComponentInParent<CompartimentComponent>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.TryGetComponent(out DoorAccesser player))
        {
            player.HouseInRange = House;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent!=null && collision.transform.parent.TryGetComponent(out DoorAccesser player))
        {
            if (player.HouseInRange == House)
            {
                player.HouseInRange = null;
            }
        }
    }
    public virtual void OnUsed()
    {
        animator.CrossFade("DoorOpen", .5f);
    }
}
