using System.Text;

namespace Unite.Genome.Feed.Data.Audit;

public class SequencingDataUploadAudit
{
    public int MutationsCreated;
    public int MutationsAssociated;
    public int CopyNumberVariantsCreated;
    public int CopyNumberVariantsAssociated;
    public int StructuralVariantsCreated;
    public int StructuralVariantsAssociated;

    public HashSet<long> Mutations;
    public HashSet<long> MutationOccurrences;
    public HashSet<long> CopyNumberVariants;
    public HashSet<long> CopyNumberVariantOccurrences;
    public HashSet<long> StructuralVariants;
    public HashSet<long> StructuralVariantOccurrences;

    public SequencingDataUploadAudit()
    {
        Mutations = new HashSet<long>();
        MutationOccurrences = new HashSet<long>();
        CopyNumberVariants = new HashSet<long>();
        CopyNumberVariantOccurrences = new HashSet<long>();
        StructuralVariants = new HashSet<long>();
        StructuralVariantOccurrences = new HashSet<long>();
    }

    public override string ToString()
    {
        var message = new StringBuilder();

        message.AppendLine($"{MutationsCreated} mutations created");
        message.AppendLine($"{MutationsAssociated} mutations associations created");
        message.AppendLine($"{CopyNumberVariantsCreated} copy number variants created");
        message.AppendLine($"{CopyNumberVariantsAssociated} copy number variants associations created");
        message.AppendLine($"{StructuralVariantsCreated} structural variants created");
        message.Append($"{StructuralVariantsAssociated} structural variants associations created");

        return message.ToString();
    }
}
