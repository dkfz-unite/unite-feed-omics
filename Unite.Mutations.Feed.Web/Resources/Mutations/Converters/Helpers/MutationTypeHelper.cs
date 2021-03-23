using System;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Web.Resources.Mutations.Converters.Helpers
{
    public class MutationTypeHelper
    {
        /// <summary>
        /// Indentifies mutation type based on reference and alternate base.
        /// </summary>
        /// <param name="referenceBase">Reference base</param>
        /// <param name="alternateBase">Alternate base</param>
        /// <returns>Mutation type (SNV, INS, DEL or MNV).</returns>
        public static MutationType GetMutationType(string referenceBase, string alternateBase)
        {
            if (!string.IsNullOrWhiteSpace(referenceBase) && !string.IsNullOrWhiteSpace(alternateBase))
            {
                if (referenceBase.Length == 1 && alternateBase.Length == 1)
                {
                    return MutationType.SNV;
                }
                else if (referenceBase.Length == 1 && alternateBase.Length > 1)
                {
                    return MutationType.INS;
                }
                else if (referenceBase.Length > 1 && alternateBase.Length == 1)
                {
                    return MutationType.DEL;
                }
                else if (referenceBase.Length > 1 && alternateBase.Length > 1)
                {
                    return MutationType.MNV;
                }
            }
            else if (string.IsNullOrWhiteSpace(referenceBase) && !string.IsNullOrWhiteSpace(alternateBase))
            {
                return MutationType.INS;
            }
            else if (!string.IsNullOrWhiteSpace(referenceBase) && string.IsNullOrWhiteSpace(alternateBase))
            {
                return MutationType.DEL;
            }

            throw new NotImplementedException($"Can't detect mutation type: REF - '{referenceBase}', ALT - '{alternateBase}'");
        }
    }
}
