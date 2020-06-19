using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableCutUpEvent : AObserverEvent
{
    public readonly CuttableFood cuttable;
    public readonly bool isHard;

    public CuttableCutUpEvent(CuttableFood cuttable, bool isHard) : base(cuttable)
    {
        this.cuttable = cuttable;
        this.isHard = isHard;
    }
}
