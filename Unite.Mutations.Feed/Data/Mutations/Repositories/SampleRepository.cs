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

            if (specimen == null)
            {
                return null;
            }

            var sample = _dbContext.Samples.FirstOrDefault(sample =>
                sample.SpecimenId == specimen.Id &&
                sample.ReferenceId == sampleModel.ReferenceId
            );

            return sample;
        }

        public Sample Create(SampleModel sampleModel)
        {
            var specimen = _specimenRepository.FindOrCreate(sampleModel.Specimen);

            var sample = new Sample
            {
                SpecimenId = specimen.Id,
                ReferenceId = sampleModel.ReferenceId
            };

            _dbContext.Samples.Add(sample);
            _dbContext.SaveChanges();

            return sample;
        }
    }
}
