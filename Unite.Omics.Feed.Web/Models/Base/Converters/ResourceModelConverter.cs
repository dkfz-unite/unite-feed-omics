using Unite.Data.Constants;

using DataModels = Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public class ResourceModelConverter
{
    public DataModels.ResourceModel Convert(ResourceModel resourceModel)
    {
        return new DataModels.ResourceModel
        {
            Name = resourceModel.Name,
            Type = resourceModel.Type,
            Format = resourceModel.Format,
            Archive = GetArchive(resourceModel.Name),
            Url = resourceModel.Url
        };
    }

    private static string GetArchive(string name)
    {
        var comparison = StringComparison.InvariantCultureIgnoreCase;

        if (name.EndsWith(".zip", comparison))
            return ArchiveTypes.Zip;
        else if (name.EndsWith(".gz", comparison))
            return ArchiveTypes.Gz;
        else
            return null;
    }
}
