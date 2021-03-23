using System.Collections.Generic;
using System.Text;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep.Audit
{
    public class AnnotationsUploadAudit
    {
        public int BiotypesCreated;
        public int GenesCreated;
        public int TranscriptsCreated;
        public int AffectedTranscriptsCreated;
        public int AffectedTranscriptConsequencesAssociated;

        public HashSet<int> Mutations;

        public AnnotationsUploadAudit()
        {
            Mutations = new HashSet<int>();
        }

        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{BiotypesCreated} biotypes created");
            message.AppendLine($"{GenesCreated} genes created");
            message.AppendLine($"{TranscriptsCreated} transcripts created");
            message.AppendLine($"{AffectedTranscriptsCreated} affected transcripts created");
            message.Append($"{AffectedTranscriptConsequencesAssociated} affected transcript consequences associated");
            
            return message.ToString();
        }
    }
}
