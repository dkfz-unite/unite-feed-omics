using System.Collections.Generic;
using System.Text;

namespace Unite.Mutations.Feed.Data.Mutations.Models.Audit
{
    public class MutationsUploadAudit
    {   
        public int MutationsCreated;
        public int MutationsAssociated;

        public HashSet<long> Mutations;
        public HashSet<long> MutationOccurrences;

        public MutationsUploadAudit()
        {
            Mutations = new HashSet<long>();
            MutationOccurrences = new HashSet<long>();
        }

        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{MutationsCreated} mutations created");
            message.Append($"{MutationsAssociated} mutations associations created");

            return message.ToString();
        }
    }
}
