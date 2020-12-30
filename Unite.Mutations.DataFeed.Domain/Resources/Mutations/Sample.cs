using System;
using Unite.Data.Entities.Samples.Enums;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class Sample
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public SampleType? Type { get; set; }
        public SampleSubtype? Subtype { get; set; }
        public DateTime? Date { get; set; }


        public void Sanitise()
        {
            Name = Name?.Trim();
            Link = Link?.Trim();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Sample;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Name == other.Name && Type == other.Type && Subtype == other.Subtype;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, Subtype);
        }
    }
}
