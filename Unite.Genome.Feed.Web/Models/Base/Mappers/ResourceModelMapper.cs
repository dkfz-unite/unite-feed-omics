using DataModels = Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Web.Models.Base.Mappers;

public class ResourceModelMapper
{
    public static void Map(in ResourceModel source, DataModels.ResourceModel target)
    {
        target.Type = source.Type;
        target.Path = source.Path;
    }
}
