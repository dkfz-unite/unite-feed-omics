namespace Unite.Mutations.Feed.Annotations
{
    public interface IAnnotationApiClient<T> where T : class
    {
        T GetAnnotations(string hgvsCode);
        T[] GetAnnotations(string[] hgvsCodes);
    }
}
