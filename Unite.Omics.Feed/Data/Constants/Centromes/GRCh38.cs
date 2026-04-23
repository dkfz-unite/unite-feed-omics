using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Constants.Centromes;

public static class GRCh38
{
    public static readonly Dictionary<Chromosome, (int Start, int End)> Values = new()
    {
        { Chromosome.Chr1, (121700000, 125100000) },
        { Chromosome.Chr2, (91800000, 96000000) },
        { Chromosome.Chr3, (87800000, 94000000) },
        { Chromosome.Chr4, (48200000, 51800000) },
        { Chromosome.Chr5, (46100000, 51400000) },
        { Chromosome.Chr6, (58500000, 62600000) },
        { Chromosome.Chr7, (58100000, 62100000) },
        { Chromosome.Chr8, (43200000, 47200000) },
        { Chromosome.Chr9, (42200000, 45500000) },
        { Chromosome.Chr10, (38000000, 41600000) },
        { Chromosome.Chr11, (51000000, 55800000) },
        { Chromosome.Chr12, (33200000, 37800000) },
        { Chromosome.Chr13, (16500000, 18900000) },
        { Chromosome.Chr14, (16100000, 18200000) },
        { Chromosome.Chr15, (17500000, 20500000) },
        { Chromosome.Chr16, (35300000, 38400000) },
        { Chromosome.Chr17, (22700000, 27400000) },
        { Chromosome.Chr18, (15400000, 21500000) },
        { Chromosome.Chr19, (24200000, 28100000) },
        { Chromosome.Chr20, (25700000, 30400000) },
        { Chromosome.Chr21, (10900000, 13000000) },
        { Chromosome.Chr22, (13700000, 17400000) },
        { Chromosome.ChrX, (58100000, 63800000) },
        { Chromosome.ChrY, (10300000, 10500000) }
    };
}
