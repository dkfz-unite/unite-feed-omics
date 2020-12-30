namespace Unite.Mutations.DataFeed.Domain.Resources.Samples
{
    public class Gene
    {
        public string Name { get; set; }


        public void Sanitise()
        {
            Name = Name?.Trim();
        }
    }
}
