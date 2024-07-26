using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        WorldController.active.EffectPool.DeactivateObject(gameObject);
    }
}

