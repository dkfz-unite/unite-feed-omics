using System;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
{
    public class File
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public void Sanitise()
        {
            Name = Name?.Trim();
            Link = Link?.Trim();
        }
    }
}
