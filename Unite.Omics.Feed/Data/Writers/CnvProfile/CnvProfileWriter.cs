using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Writers.CnvProfile;

public class CnvProfileWriter: DataWriter<SampleModel, AnalysisWriteAudit>
{
    public CnvProfileWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        throw new NotImplementedException();
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        throw new NotImplementedException();
    }
}