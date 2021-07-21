using System.Linq;
using Unite.Data.Entities;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
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

            // Analysis type should match
            query = query.Where(analysis =>
                analysis.TypeId == analysisModel.Type
            );

            // Number of analysed samples should match
            query = query.Where(analysis =>
                analysis.AnalysedSamples.Count() == analysisModel.AnalysedSamples.Count()
            );

            // Each analysed sample should match
            foreach (var analysedSampleModel in analysisModel.AnalysedSamples)
            {
                if (analysedSampleModel.MatchedSample == null)
                {
                    // Analysed sample should match
                    var sample = _sampleRepository.Find(analysedSampleModel.AnalysedSample);

                    if (sample == null)
                    {
                        return null;
                    }
                    else
                    {
                        query = query.Where(analysis =>
                            analysis.AnalysedSamples.Any(analysedSample =>
                                analysedSample.SampleId == sample.Id &&
                                analysedSample.MatchedSampleId == null
                            )
                        );
                    }
                }
                else
                {
                    // Analysed sample should match
                    var sample = _sampleRepository.Find(analysedSampleModel.AnalysedSample);

                    // Matched sample should match
                    var matchedSample = _sampleRepository.Find(analysedSampleModel.MatchedSample);

                    if (sample == null || matchedSample == null)
                    {
                        return null;
                    }
                    else
                    {
                        query = query.Where(analysis =>
                            analysis.AnalysedSamples.Any(analysedSample =>
                                analysedSample.SampleId == sample.Id &&
                                analysedSample.MatchedSampleId == matchedSample.Id
                            )
                        );
                    }
                }
            }

            return query.FirstOrDefault();
        }

        public Analysis Create(AnalysisModel analysisModel)
        {
            var analysis = new Analysis();

            Map(analysisModel, ref analysis);

            _dbContext.Analyses.Add(analysis);
            _dbContext.SaveChanges();

            return analysis;
        }


        private void Map(in AnalysisModel model, ref Analysis analysis)
        {
            analysis.TypeId = model.Type;

            if (model.File != null)
            {
                if (analysis.File == null)
                {
                    analysis.File = new File();
                }

                analysis.File.Name = model.File.Name;
                analysis.File.Link = model.File.Link;
            }
        }
    }
}
