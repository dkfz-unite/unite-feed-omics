using Unite.Data.Entities.Omics.Enums;
using Unite.Omics.Feed.Data.Constants;
using Unite.Omics.Feed.Data.Constants.Centromes;

namespace Unite.Omics.Feed.Data.Helpers;

public static class ChromosomeArmDetector
{
    public static ChromosomeArm? Detect(Chromosome chromosome, int start, int end, string genome)
    {
        var centromes = GetCentromes(genome);

        if (!centromes.TryGetValue(chromosome, out var centrome))
            return null;

        if (end <= centrome.Start)
            return ChromosomeArm.P;
        else if (start >= centrome.End)
            return ChromosomeArm.Q;
        else
            return null;
    }

    private static Dictionary<Chromosome, (int Start, int End)> GetCentromes(string genome)
    {
        return genome switch
        {
            Genomes.GRCh37 => GRCh37.Values,
            Genomes.GRCh38 => GRCh38.Values,
            _ => throw new ArgumentException($"Unsupported genome: {genome}")
        };
    }
}
