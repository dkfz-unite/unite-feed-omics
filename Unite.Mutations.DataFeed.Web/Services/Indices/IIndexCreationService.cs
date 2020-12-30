using Unite.Indices.Entities.Mutations;

namespace Unite.Mutations.DataFeed.Web.Services.Indices
{
    public interface IIndexCreationService
    {
        MutationIndex CreateIndex(int mutationId);
    }
}