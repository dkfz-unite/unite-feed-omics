using Unite.Data.Entities.Mutations;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Feed.Data.Services.Annotations.Models.Vep
{
    public class ConsequenceModel
    {
        public ConsequenceType Type;

        public Consequence ToEntity()
        {
            var consequence = new Consequence
            {
                TypeId = Type
            };

            return consequence;
        }
    }
}
