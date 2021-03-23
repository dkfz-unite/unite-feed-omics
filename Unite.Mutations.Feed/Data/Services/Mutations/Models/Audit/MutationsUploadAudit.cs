using System.Collections.Generic;
using System.Text;

namespace Unite.Mutations.Feed.Data.Services.Mutations.Models.Audit
{
    public class MutationsUploadAudit
    {
        public int DonorsCreated;
        public int AnalysesCreated;
        public int SamplesCreated;
        public int SamplesAnalysed;
        public int SamplesMatched;
        public int MutationsCreated;
        public int MutationsAssociated;

        public HashSet<int> Mutations;

        public MutationsUploadAudit()
        {
            Mutations = new HashSet<int>();
        }

        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{DonorsCreated} donors created");
            message.AppendLine($"{AnalysesCreated} analyses created");
            message.AppendLine($"{SamplesCreated} samples created");
            message.AppendLine($"{SamplesAnalysed} samples analysed");
            message.AppendLine($"{SamplesMatched} samples matched");
            message.AppendLine($"{MutationsCreated} mutations created");
            message.Append($"{MutationsAssociated} mutations associations created");

            return message.ToString();
        }
    }
}
