using System.Collections.Generic;
using System.Text;

namespace Unite.Mutations.Feed.Mutations.Data.Models.Audit
{
    public class MutationsUploadAudit
    {   
        public int MutationsCreated;
        public int MutationsAssociated;

        public HashSet<long> Mutations;

        public MutationsUploadAudit()
        {
            Mutations = new HashSet<long>();
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
