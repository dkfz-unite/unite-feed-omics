using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class AffectedTranscriptConsequenceRepository : Repository<AffectedTranscriptConsequence>
    {
        public AffectedTranscriptConsequenceRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in AffectedTranscriptConsequence source, ref AffectedTranscriptConsequence target)
        {
            target.AffectedTranscriptId = source.AffectedTranscript?.Id ?? source.AffectedTranscriptId;
            target.ConsequenceId = source.Consequence?.TypeId ?? source.ConsequenceId;
        }
    }
}
