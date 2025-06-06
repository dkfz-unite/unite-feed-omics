namespace Unite.Omics.Feed.Web.Models.Base.Converters;
using DataModels = Unite.Omics.Feed.Data.Models;

public class ResourceModelConverter
{
    public DataModels.ResourceModel Convert(ResourceModel resourceModel)
    {
        return new DataModels.ResourceModel
        {
            Name = resourceModel.Name,
            Type = resourceModel.Type,
            Format = resourceModel.Format,
            Archive = resourceModel.Archive,
            Url = resourceModel.Url
        };
    }
}
