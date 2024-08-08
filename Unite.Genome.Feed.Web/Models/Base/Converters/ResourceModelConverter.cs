namespace Unite.Genome.Feed.Web.Models.Base.Converters;
using DataModels = Unite.Genome.Feed.Data.Models;

public class ResourceModelConverter
{
    public DataModels.ResourceModel Convert(ResourceModel resourceModel)
    {
        return new DataModels.ResourceModel
        {
            Type = resourceModel.Type,
            Format = resourceModel.Format,
            Url = resourceModel.Url
        };
    }
}
