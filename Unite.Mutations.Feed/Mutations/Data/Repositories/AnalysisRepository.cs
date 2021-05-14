using System.Linq;
using Unite.Data.Entities;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Mutations.Data.Models;

namespace Unite.Mutations.Feed.Mutations.Data.Repositories
{
    internal class AnalysisRepository
    {
        private readonly UniteDbContext _dbContext;
        private readonly SampleRepository _sampleRepository;


        public AnalysisRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _sampleRepository = new SampleRepository(dbContext);
        }


        public Analysis FindOrCreate(AnalysisModel analysisModel)
        {
            return Find(analysisModel) ?? Create(analysisModel);
        }

        public Analysis Find(AnalysisModel analysisModel)
        {
            var query = _dbContext.Analyses.AsQueryable();

            query = query.Where(analysis =>
                analysis.TypeId == analysisModel.Type
            );

            query = query.Where(analysis =>
                analysis.Date == analysisModel.Date
            );

            query = query.Where(analysis =>
                analysis.AnalysedSamples.Count() == analysisModel.AnalysedSamples.Count()
            );

            foreach (var analysedSampleModel in analysisModel.AnalysedSamples)
            {
                var sample = _sampleRepository.Find(analysedSampleModel);

                if(sample == null)
                {
                    return null;
                }
                else
                {
                    query = query.Where(analysis =>
                        analysis.AnalysedSamples.Any(analysedSample =>
                            analysedSample.SampleId == sample.Id
                        )
                    );
                }
            }

            return query.FirstOrDefault();
        }

        public Analysis Create(AnalysisModel analysisModel)
        {
            var analysis = new Analysis();

            analysis.TypeId = analysisModel.Type;
            analysis.Date = analysisModel.Date;

            if (analysisModel.File != null)
            {
                analysis.File = new File();

                analysis.File.Name = analysisModel.File.Name;
                analysis.File.Link = analysisModel.File.Link;
                analysis.File.Created = analysisModel.File.Created;
                analysis.File.Updated = analysisModel.File.Updated;
            }

            _dbContext.Analyses.Add(analysis);
            _dbContext.SaveChanges();

            return analysis;
        }
    }
}
