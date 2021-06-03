using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    internal class SampleRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly SpecimenRepository _specimenRepository;


        public SampleRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _specimenRepository = new SpecimenRepository(dbContext);
        }


        public Sample FindOrCreate(SampleModel sampleModel)
        {
            return Find(sampleModel) ?? Create(sampleModel);
        }

        public Sample Find(SampleModel sampleModel)
        {
            var specimen = _specimenRepository.Find(sampleModel.Specimen);

            if(specimen == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(sampleModel.ReferenceId))
            {
                return FindByReference(specimen.Id, sampleModel.ReferenceId);
            }
            else
            {
                return FindByModel(specimen.Id, sampleModel);
            }
        }

        public Sample Create(SampleModel sampleModel)
        {
            var specimen = _specimenRepository.FindOrCreate(sampleModel.Specimen);

            var sample = new Sample
            {
                SpecimenId = specimen.Id
            };

            Map(sampleModel, sample);

            _dbContext.Samples.Add(sample);
            _dbContext.SaveChanges();

            return sample;
        }


        private Sample FindByReference(int specimenId, string referenceId)
        {
            var sample = _dbContext.Samples.FirstOrDefault(sample =>
                sample.SpecimenId == specimenId &&
                sample.ReferenceId == referenceId
            );

            return sample;
        }

        private Sample FindByModel(int specimenId, SampleModel sampleModel)
        {
            var sample = _dbContext.Samples.FirstOrDefault(sample =>
                sample.SpecimenId == specimenId &&
                sample.Date == sampleModel.Date
            );

            return sample;
        }

        private void Map(SampleModel sampleModel, Sample sample)
        {
            sample.ReferenceId = sampleModel.ReferenceId;

            sample.Date = sampleModel.Date;
        }
    }
}
