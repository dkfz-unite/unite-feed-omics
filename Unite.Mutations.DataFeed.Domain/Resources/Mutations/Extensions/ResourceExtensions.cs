using System;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Extensions
{
    public static class ResourceExtensions
    {
        public static Data.Entities.Donors.Donor GetDonor(this Resource resource)
        {
            var donor = new Data.Entities.Donors.Donor();

            donor.Id = resource.Pid;

            return donor;
        }


        public static Data.Entities.Mutations.Analysis GetAnalysis(this Resource resource, string pid)
        {
            var analysis = new Data.Entities.Mutations.Analysis();

            if(resource.Analysis != null)
            {
                analysis.DonorId = pid;
                analysis.Name = resource.Analysis.Name;
                analysis.TypeId = resource.Analysis.Type;
                analysis.Date = resource.Analysis.Date;
                analysis.File = GetFile(resource.Analysis.File);
            }
            else
            {
                analysis.DonorId = pid;
                analysis.Name = Guid.NewGuid().ToString();
            }

            return analysis;
        }

        private static Data.Entities.File GetFile(File fileResource)
        {
            if(fileResource == null)
            {
                return null;
            }

            var file = new Data.Entities.File();

            file.Name = fileResource.Name;
            file.Link = fileResource.Link;
            file.Created = fileResource.Created;
            file.Updated = fileResource.Updated;

            return file;
        }


        public static Data.Entities.Mutations.Sample GetSample(this Sample sampleResource, string pid)
        {
            var sample = new Data.Entities.Mutations.Sample();

            sample.DonorId = pid;
            sample.Name = sampleResource.Name;
            sample.TypeId = sampleResource.Type;
            sample.SubtypeId = sampleResource.Subtype;
            sample.Date = sampleResource.Date;

            return sample;
        }


        public static Data.Entities.Mutations.Mutation GetMutation(this Mutation mutationResource)
        {
            var mutation = new Data.Entities.Mutations.Mutation();

            mutation.Code = mutationResource.Code;
            mutation.ChromosomeId = mutationResource.Chromosome;
            mutation.SequenceTypeId = mutationResource.SequenceType;
            mutation.Start = mutationResource.Start;
            mutation.End = mutationResource.End;
            mutation.TypeId = mutationResource.Type;
            mutation.ReferenceBase = mutationResource.Ref;
            mutation.AlternateBase = mutationResource.Alt;

            return mutation;
        }
    }
}
