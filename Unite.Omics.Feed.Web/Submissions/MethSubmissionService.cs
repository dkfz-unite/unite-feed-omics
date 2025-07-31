using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Submissions;

public class MethSubmissionService
{
    private readonly SampleSubmissionRepository _sampleRepository;
    private readonly LevelSubmissionRepository _levelRepository;


    public MethSubmissionService(IMongoOptions options)
    {
        _sampleRepository = new SampleSubmissionRepository(options);
        _levelRepository = new LevelSubmissionRepository(options);   
    }


    public string AddSampleSubmission(SampleModel model)
    {
        return _sampleRepository.Add(model);
    }

    public string AddLevelSubmission(AnalysisModel<EmptyModel> model)
    {
        return _levelRepository.Add(model);
    }

    public SampleModel FindSampleSubmission(string id)
    {
        return _sampleRepository.Find(id)?.Document;
    }

    public AnalysisModel<EmptyModel> FindLevelSubmission(string id)
    {
        return _levelRepository.Find(id)?.Document;
    }

    public void DeleteSampleSubmission(string id)
    {
        _sampleRepository.Delete(id);
    }

    public void DeleteLevelSubmission(string id)
    {
        _levelRepository.Delete(id);
    }
}
