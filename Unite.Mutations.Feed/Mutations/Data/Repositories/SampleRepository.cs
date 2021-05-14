using System.Linq;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
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
            if (!string.IsNullOrWhiteSpace(sampleModel.ReferenceId))
            {
                return Find(sampleModel.ReferenceId);
            }
            else
            {
                var specimen = _specimenRepository.Find(sampleModel.Specimen);

                if (specimen != null)
                {
                    return Find(specimen.Id, sampleModel);
                }
            }

            return null;
        }

        public Sample Create(SampleModel sampleModel)
        {
            var specimen = _specimenRepository.FindOrCreate(sampleModel.Specimen);

            return Create(specimen.Id, sampleModel);
        }


        private Sample Find(string referenceId)
        {
            var sample = _dbContext.Samples.FirstOrDefault(sample =>
                sample.ReferenceId == referenceId
            );

            return sample;
        }

        private Sample Find(int specimenId, SampleModel sampleModel)
        {
            var sample = _dbContext.Samples.FirstOrDefault(sample =>
                sample.SpecimenId == specimenId &&
                sample.Date == sampleModel.Date
            );

            return sample;
        }

        private Sample Create(int specimenId, SampleModel sampleModel)
        {
            var sample = new Sample();

            sample.ReferenceId = sampleModel.ReferenceId;
            sample.Date = sampleModel.Date;

            sample.SpecimenId = specimenId;

            _dbContext.Samples.Add(sample);
            _dbContext.SaveChanges();

            return sample;
        }
    }
}
