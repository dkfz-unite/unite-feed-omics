using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;

namespace Unite.Genome.Feed.Web.Services;

public class MutationIndexingTaskService : IndexingTaskService<Mutation, long>
{
    protected override int BucketSize => 1000;


    public MutationIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }


    public override void CreateTasks()
    {
        IterateEntities<Mutation, long>(mutation => true, mutation => mutation.Id, mutations =>
        {
            CreateMutationIndexingTasks(mutations);
        });
    }

    public override void CreateTasks(IEnumerable<long> mutationIds)
    {
        IterateEntities<Mutation, long>(mutation => mutationIds.Contains(mutation.Id), mutation => mutation.Id, mutations =>
        {
            CreateMutationIndexingTasks(mutations);
        });
    }

    public override void PopulateTasks(IEnumerable<long> mutationIds)
    {
        IterateEntities<Mutation, long>(mutation => mutationIds.Contains(mutation.Id), mutation => mutation.Id, mutations =>
        {
            CreateDonorIndexingTasks(mutations);
            CreateImageIndexingTasks(mutationIds);
            CreateSpecimenIndexingTasks(mutations);
            CreateMutationIndexingTasks(mutations);
            CreateGeneIndexingTasks(mutations);
        });
    }


    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<long> keys)
    {
        var donorIds = _dbContext.Set<MutationOccurrence>()
            .Where(mutationOccurrence => keys.Contains(mutationOccurrence.MutationId))
            .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId)
            .Distinct()
            .ToArray();

        return donorIds;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<long> keys)
    {
        var donorIds = _dbContext.MutationOccurrences
            .Where(mutationOccurrence => keys.Contains(mutationOccurrence.MutationId))
            .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId)
            .Distinct()
            .ToArray();

        var imageIds = _dbContext.Set<Image>()
            .Where(image => donorIds.Contains(image.DonorId))
            .Select(image => image.Id)
            .Distinct()
            .ToArray();

        return imageIds;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<long> keys)
    {
        var specimenIds = _dbContext.MutationOccurrences
            .Where(mutationOccurrence => keys.Contains(mutationOccurrence.MutationId))
            .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        return specimenIds;
    }

    protected override IEnumerable<long> LoadRelatedMutations(IEnumerable<long> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<long> keys)
    {
        var geneIds = _dbContext.AffectedTranscripts
            .Where(affectedTranscript => keys.Contains(affectedTranscript.MutationId))
            .Where(affectedTranscript => affectedTranscript.Transcript.GeneId != null)
            .Select(affectedTranscript => affectedTranscript.Transcript.GeneId.Value)
            .Distinct()
            .ToArray();

        return geneIds;
    }
}
