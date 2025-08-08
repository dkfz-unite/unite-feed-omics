namespace Unite.Omics.Feed.Web.Models.Base.Extensions;

public static class FormFileExtensions
{
    public static bool IsEmpty(this IFormFile file)
    {
        return file == null || file.Length == 0;
    }
}
