namespace Unite.Genome.Feed.Data.Writers;

public abstract class DataWriteAudit
{
    public int ResourcesCreated;
    public int ResourcesUpdated;

    public HashSet<int> Samples = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{ResourcesCreated} resources created",
            $"{ResourcesUpdated} resources updated",
        ]);
    }
}
