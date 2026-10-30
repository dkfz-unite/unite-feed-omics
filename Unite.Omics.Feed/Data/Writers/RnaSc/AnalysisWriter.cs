using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Writers.RnaSc;

public class AnalysisWriter : Writers.AnalysisWriter<AnalysisModel, AnalysisWriteAudit>
{
    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
