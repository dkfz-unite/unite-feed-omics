using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Models.Base.Helpers;

public static class ChromosomeParser
{
    public static bool TryParse(string source, out Chromosome? result)
    {
        var values = Enum.GetValues(typeof(Chromosome)).Cast<Chromosome>().ToArray();

        foreach (var value in values)
        {
            var incoming = source.Trim();
            var desiredRaw = value.ToDefinitionString();
            var desiredPrefixed = $"chr{desiredRaw}";
            var desiredMT = value == Chromosome.ChrMT ? "chrM" : null;

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            if (string.Equals(incoming, desiredRaw, comparison) ||
                string.Equals(incoming, desiredPrefixed, comparison) ||
                string.Equals(incoming, desiredMT,comparison))
            {
                result = value;
                return true;
            }
        }

        result = null;
        return false;
    }
}
