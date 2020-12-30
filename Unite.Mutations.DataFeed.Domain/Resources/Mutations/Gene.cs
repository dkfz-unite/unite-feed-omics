namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations
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
