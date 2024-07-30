namespace Unite.Genome.Feed.Data.Writers;

public abstract class DataWriteAudit
{
    public int SamplesCreated;
    public int SamplesUpdated;
    public int ResourcesCreated;
    public int ResourcesUpdated;

    public HashSet<int> Samples = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{SamplesCreated} samples created",
            $"{SamplesUpdated} samples updated",
            $"{ResourcesCreated} resources created",
            $"{ResourcesUpdated} resources updated",
        ]);
    }
}
