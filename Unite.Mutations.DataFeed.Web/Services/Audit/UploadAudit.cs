using System.Text;

namespace Unite.Mutations.DataFeed.Web.Services.Audit
{
    public class UploadAudit
    {
        public int DonorsCreated;
        public int SamplesCreated;
        public int MutationsCreated;
        public int MutationsAssociated;

        public override string ToString()
        {
            var message = new StringBuilder();

            message.AppendLine($"{DonorsCreated} new donors created");
            message.AppendLine($"{SamplesCreated} new samples created");
            message.AppendLine($"{MutationsCreated} new mutations created");
            message.Append($"{MutationsAssociated} new sample mutation associations created");

            return message.ToString();
        }
    }
}
