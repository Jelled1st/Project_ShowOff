public interface IScoresObserver
{
    void AddedScore(float score);
    void SubtractedScore(float score);
    void AdjustedScore(float score);

    void Subscribe();
    void UnSubscribe();
}
