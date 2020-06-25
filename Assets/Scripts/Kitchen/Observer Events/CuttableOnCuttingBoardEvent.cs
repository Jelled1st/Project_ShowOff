public class CuttableOnCuttingBoardEvent : AObserverEvent
{
    public readonly CuttingBoard board;
    public readonly CuttableFood food;

    public CuttableOnCuttingBoardEvent(CuttingBoard cuttingBoard, CuttableFood food) : base(cuttingBoard)
    {
        board = cuttingBoard;
        this.food = food;
    }
}