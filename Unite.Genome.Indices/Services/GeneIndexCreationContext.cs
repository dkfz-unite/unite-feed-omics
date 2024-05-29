using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Genome.Indices.Services;

internal class GeneIndexCreationContext
{
    /// <summary>
    /// Gene.
    /// </summary>
    public Gene Gene;

    /// <summary>
    /// Map of variant id to affected transcripts.
    /// </summary>
    public IDictionary<int, SSM.AffectedTranscript[]> SsmAffectedTranscriptsCache;

    /// <summary>
    /// Map of variant id to affected transcripts.
    /// </summary>
    public IDictionary<int, CNV.AffectedTranscript[]> CnvAffectedTranscriptsCache;

    /// <summary>
    /// Map of variant id to affected transcripts.
    /// </summary>
    public IDictionary<int, SV.AffectedTranscript[]> SvAffectedTranscriptsCache;

    /// <summary>
    /// Map of specimen id to specimen. There can be one specimen per specimen.
    /// </summary>
    public IDictionary<int, Specimen> SpecimensCache;

    /// <summary>
    /// Map of specimen id to donor. There can be one donor per specimen.
    /// </summary>
    public IDictionary<int, Donor> DonorsCache;

    /// <summary>
    /// Map of specimen id to gene expression. There can be one gene expression per specimen.
    /// </summary>
    public IDictionary<int, GeneExpression> GeneExpressionsCache;

    /// <summary>
    /// Map of specimen id to images. There can be multiple images per specimen.
    /// </summary>
    public IDictionary<int, Image[]> ImagesCache;

    /// <summary>
    /// Map of specimen id to samples. There can be multiple samples of one type per specimen.
    /// </summary>
    public IDictionary<int, Sample[]> Samples;
}
