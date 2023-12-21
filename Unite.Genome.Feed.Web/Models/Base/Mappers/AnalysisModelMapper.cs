using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Mappers;

public class AnalysisModelMapper
{
    public static void Map(in AnalysisModel source, DataModels.AnalysisModel target)
    {
        target.ReferenceId = source.Id;
        target.Type = source.Type;
        target.Date = source.Date;
        target.Day = source.Day;
        target.Parameters = source.Parameters;
    }
}
