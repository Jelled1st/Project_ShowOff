using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableCutEvent : AObserverEvent
{
    public readonly CuttableFood cuttable;
    public readonly bool isHard;
    public readonly int state;
    public readonly bool done;

    public CuttableCutEvent(CuttableFood cuttable, bool isHard, int state, bool done) : base(cuttable)
    {
        this.cuttable = cuttable;
        this.isHard = isHard;
        this.state = state;
        this.done = done;
    }
}
