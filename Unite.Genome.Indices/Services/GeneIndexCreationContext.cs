using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;

using CNV = Unite.Data.Entities.Genome.Variants.CNV;
using SSM = Unite.Data.Entities.Genome.Variants.SSM;
using SV = Unite.Data.Entities.Genome.Variants.SV;

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
    public IDictionary<long, SSM.AffectedTranscript[]> SsmAffectedTranscriptsCache;

    /// <summary>
    /// Map of variant id to affected transcripts.
    /// </summary>
    public IDictionary<long, CNV.AffectedTranscript[]> CnvAffectedTranscriptsCache;

    /// <summary>
    /// Map of variant id to affected transcripts.
    /// </summary>
    public IDictionary<long, SV.AffectedTranscript[]> SvAffectedTranscriptsCache;

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
    public IDictionary<int, BulkExpression> BulkExpressionsCache;

    /// <summary>
    /// Map of specimen id to images. There can be multiple images per specimen.
    /// </summary>
    public IDictionary<int, Image[]> ImagesCache;

    /// <summary>
    /// Map of specimen id to analyses. There can be multiple analyses per specimen.
    /// </summary>
    public IDictionary<int, AnalysedSample[]> AnalysesCache;
}
