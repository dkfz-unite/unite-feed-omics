using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Constants.Centromes;

public static class GRCh37
{
    public static readonly Dictionary<Chromosome, (int Start, int End)> Values = new()
    {
        { Chromosome.Chr1, (121535434, 124535434) },
        { Chromosome.Chr2, (92326171, 95326171) },
        { Chromosome.Chr3, (90504854, 93504854) },
        { Chromosome.Chr4, (49660117, 52660117) },
        { Chromosome.Chr5, (46405641, 49405641) },
        { Chromosome.Chr6, (58830166, 61830166) },
        { Chromosome.Chr7, (58054331, 61054331) },
        { Chromosome.Chr8, (43838887, 46838887) },
        { Chromosome.Chr9, (47367679, 50367679) },
        { Chromosome.Chr10, (39254935, 42254935) },
        { Chromosome.Chr11, (51644205, 54644205) },
        { Chromosome.Chr12, (34856694, 37856694) },
        { Chromosome.Chr13, (16000000, 19000000) },
        { Chromosome.Chr14, (16000000, 19000000) },
        { Chromosome.Chr15, (17000000, 20000000) },
        { Chromosome.Chr16, (35335801, 38335801) },
        { Chromosome.Chr17, (22263006, 25263006) },
        { Chromosome.Chr18, (15460898, 18460898) },
        { Chromosome.Chr19, (24681782, 27681782) },
        { Chromosome.Chr20, (26369569, 29369569) },
        { Chromosome.Chr21, (11288129, 14288129) },
        { Chromosome.Chr22, (13000000, 16000000) },
        { Chromosome.ChrX, (58632012, 61632012) },
        { Chromosome.ChrY, (10104553, 13104553) }
    };
}
