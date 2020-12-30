namespace Unite.Mutations.DataFeed.Domain.Resources.Samples.Extensions
{
    public static class ResourceExtensions
    {
        public static Data.Entities.Donors.Donor GetDonor(this Sample sampleResource)
        {
            var donor = new Data.Entities.Donors.Donor();

            donor.Id = sampleResource.Pid;

            return donor;
        }

        public static Data.Entities.Samples.Sample GetSample(this Sample sampleResource)
        {
            var sample = new Data.Entities.Samples.Sample();

            sample.DonorId = sampleResource.Pid;
            sample.Name = sampleResource.Name;
            sample.Link = sampleResource.Link;
            sample.TypeId = sampleResource.Type;
            sample.SubtypeId = sampleResource.Subtype;
            sample.Date = sampleResource.Date;

            return sample;
        }

        public static Data.Entities.Samples.SampleMutation GetSampleMutation(this Mutation mutationResource)
        {
            var sampleMutation = new Data.Entities.Samples.SampleMutation();

            sampleMutation.Quality = mutationResource.Quality;
            sampleMutation.Filter = mutationResource.Filter;
            sampleMutation.Info = mutationResource.Info;
            
            return sampleMutation;
        }

        public static Data.Entities.Mutations.Mutation GetMutation(this Mutation mutationResource)
        {
            var mutation = new Data.Entities.Mutations.Mutation();

            mutation.Name = mutationResource.Id;
            mutation.Gene = GetGene(mutationResource.Gene);
            mutation.ChromosomeId = mutationResource.Chromosome;
            mutation.Contig = GetConting(mutationResource.Contig);
            mutation.SequenceTypeId = mutationResource.SequenceType.Value;
            mutation.Position = mutationResource.Position.Value;
            mutation.TypeId = mutationResource.Type.Value;
            mutation.ReferenceAllele = mutationResource.Ref;
            mutation.AlternateAllele = mutationResource.Alt;
            mutation.Code = mutation.GetCode();

            return mutation;
        }

        private static Data.Entities.Mutations.Contig GetConting(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var contig = new Data.Entities.Mutations.Contig();

            contig.Value = value;

            return contig;
        }

        private static Data.Entities.Mutations.Gene GetGene(Gene geneResource)
        {
            if (geneResource == null)
            {
                return null;
            }

            var gene = new Data.Entities.Mutations.Gene();

            gene.Name = geneResource.Name;

            return gene;
        }
    }
}
