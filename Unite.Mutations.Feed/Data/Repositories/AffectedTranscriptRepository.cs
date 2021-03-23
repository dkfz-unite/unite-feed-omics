using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class AffectedTranscriptRepository : Repository<AffectedTranscript>
    {
        public AffectedTranscriptRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in AffectedTranscript source, ref AffectedTranscript target)
        {
            target.MutationId = source.Mutation?.Id ?? source.MutationId;
            target.GeneId = source.Gene?.Id ?? source.GeneId;
            target.TranscriptId = source.Transcript?.Id ?? source.TranscriptId;

            target.CDNAStart = source.CDNAStart;
            target.CDNAEnd = source.CDNAEnd;
            target.CDSStart = source.CDSStart;
            target.CDSEnd = source.CDSEnd;
            target.ProteinStart = source.ProteinStart;
            target.ProteinEnd = source.ProteinEnd;
            target.AminoAcidChange = source.AminoAcidChange;
            target.CodonChange = source.CodonChange;
        }
    }
}
