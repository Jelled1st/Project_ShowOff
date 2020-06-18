public class CuttableOnCuttingBoardEvent : AObserverEvent
{
    public readonly CuttingBoard board;
    public readonly CuttableFood food;
    public CuttableOnCuttingBoardEvent(CuttingBoard cuttingBoard, CuttableFood food) : base(cuttingBoard)
    {
        this.board = cuttingBoard;
        this.food = food;
    }
}
