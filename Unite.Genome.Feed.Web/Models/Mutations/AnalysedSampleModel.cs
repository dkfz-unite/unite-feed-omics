using Unite.Data.Extensions;
using Unite.Genome.Feed.Web.Services.Mutations;

namespace Unite.Genome.Feed.Web.Services.Mutations;

public class AnalysedSampleModel : SampleModel
{
    /// <summary>
    /// Matched sample id
    /// </summary>
    public string MatchedSampleId { get; set; }

    /// <summary>
    /// List of mutations appeared in the sample after analysis
    /// </summary>
    public MutationModel[] Mutations { get; set; }


    public override void Sanitise()
    {
        base.Sanitise();

        MatchedSampleId = MatchedSampleId?.Trim();

        Mutations?.ForEach(mutation => mutation.Sanitise());
    }
}
