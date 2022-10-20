using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Mappers;

public class AnalysisModelMapper
{
    public void Map(in AnalysisModel source, DataModels.AnalysisModel target)
    {
        target.ReferenceId = source.Id;
        target.Type = source.Type;
        target.Date = FromDateTime(source.Date);
        target.Parameters = source.Parameters;
    }

    private static DateOnly? FromDateTime(DateTime? date)
    {
        return date != null ? DateOnly.FromDateTime(date.Value) : null;
    }
}
