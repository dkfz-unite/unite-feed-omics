namespace Unite.Genome.Feed.Web.Services.Mutations
{
    public class FileModel
    {
        public string Name { get; set; }
        public string Link { get; set; }

        public void Sanitise()
        {
            Name = Name?.Trim();
            Link = Link?.Trim();
        }
    }
}