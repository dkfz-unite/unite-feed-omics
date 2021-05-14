using System;

namespace Unite.Mutations.Feed.Mutations.Data.Models
{
    public class FileModel
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}