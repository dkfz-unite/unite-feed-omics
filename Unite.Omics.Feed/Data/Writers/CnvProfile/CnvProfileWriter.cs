using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Repositories.CnvProfile;

namespace Unite.Omics.Feed.Data.Writers.CnvProfile;

//TODO: swap Writer and Repository concepts, Repository is actually a Writer, and Writer is actually a Repository
public class CnvProfileWriter(
    IDbContextFactory<DomainDbContext> dbContextFactory)
    : DataWriter<SampleModel, AnalysisWriteAudit>(dbContextFactory)
{
    private CnvProfileRepository _cnvProfileRepository;
    
    //TODO: pass DbContext to the DB-OP methods directly as parameter(call stack lifetime), do not recreate repositories multiple times
    protected override void Initialize(DomainDbContext dbContext)
    {
        _cnvProfileRepository = new CnvProfileRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sampleId = WriteSample(model, ref audit);

        if (model.CnvProfiles.IsNotEmpty())
        {
           var cnvProfiles = _cnvProfileRepository.CreateAll(sampleId, model.CnvProfiles);
            audit.CnvProfilesCreated += cnvProfiles.Count();
        }
    }
}