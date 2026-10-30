using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisCnvpWriter : Writers.AnalysisWriter<AnalysisModel<Models.Dna.Cnv.ProfileModel>, AnalysisWriteAudit>
{
    private Repositories.Dna.Cnv.ProfileRepository _profileRepository;


    public AnalysisCnvpWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }


    protected override void InitializeEntries(DomainDbContext dbContext)
    {
        _profileRepository = new Repositories.Dna.Cnv.ProfileRepository(dbContext);
    }

    protected override void WriteEntries(int sampleId, AnalysisModel<Models.Dna.Cnv.ProfileModel> model, ref AnalysisWriteAudit audit)
    {
        var models = model.Entries;
        if (models.IsEmpty())
            return;

        var removedProfiles = _profileRepository.RemoveAll(sampleId);
        audit.Entries.AddRange(removedProfiles);

        var profiles = _profileRepository.CreateAll(sampleId, models);
        audit.Entries.AddRange(profiles.Select(entity => entity.Id));
        audit.EntriesCreated += profiles.Length;
    }
}
