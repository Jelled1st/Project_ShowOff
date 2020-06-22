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
