using System;

namespace Unite.Mutations.Feed.Web.Resources.Mutations
{
    public class FileResource
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