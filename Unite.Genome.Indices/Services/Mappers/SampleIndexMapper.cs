using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Genome.Analysis;

namespace Unite.Genome.Indices.Services.Mappers;

internal class SampleIndexMapper
{
    internal void Map(in Sample sample, in Analysis[] analyses, SampleIndex index, DateOnly? diagnosisDate)
    {
        if (sample == null)
        {
            return;
        }

        index.Id = sample.Id;
        index.ReferenceId = sample.ReferenceId;
        index.Analyses = CreateFrom(analyses, diagnosisDate);
    }

    private AnalysisIndex[] CreateFrom(in IEnumerable<Analysis> analyses, DateOnly? diagnosisDate)
    {
        if (analyses?.Any() != true)
        {
            return null;
        }

        var indices = analyses.Select(analysis => 
        {
            var index = new AnalysisIndex();

            index.Id = analysis.Id;
            index.ReferenceId = analysis.ReferenceId;
            index.Day = analysis.Date.RelativeFrom(diagnosisDate);
            index.Type = analysis.TypeId.ToDefinitionString();

            return index;

        }).ToArray();


        return indices;
    }
}
