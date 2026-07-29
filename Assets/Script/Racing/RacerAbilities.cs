using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerAbilities : RacerComponent
{
    [NonSerialized] protected List<Ability> abilities = new List<Ability>();

    public RacerAbilities(Racer racer) : base(racer)
    {
    }
}
