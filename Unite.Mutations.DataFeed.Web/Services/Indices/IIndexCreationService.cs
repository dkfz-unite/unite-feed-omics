namespace Unite.Mutations.DataFeed.Web.Services.Indices
{
    public interface IIndexCreationService<T> where T : class
    {
        T CreateIndex(int id);
    }
}