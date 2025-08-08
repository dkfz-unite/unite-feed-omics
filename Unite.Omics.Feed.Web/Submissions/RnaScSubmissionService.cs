using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Submissions;

public class RnaScSubmissionService
{
	private readonly SampleSubmissionRepository _sampleRepository;
    private readonly ExpSubmissionRepository _expRepository;


    public RnaScSubmissionService(IMongoOptions options)
	{
		_sampleRepository = new SampleSubmissionRepository(options);
		_expRepository = new ExpSubmissionRepository(options);
	}


	public string AddSampleSubmission(SampleModel data)
	{
		return _sampleRepository.Add(data);
	}

	public string AddExpSubmission(AnalysisModel<EmptyModel> data)
	{
		return _expRepository.Add(data);
	}

	public SampleModel FindSampleSubmission(string id)
	{
		return _sampleRepository.Find(id)?.Document;
	}

	public AnalysisModel<EmptyModel> FindExpSubmission(string id)
	{
		return _expRepository.Find(id)?.Document;
	}

	public void DeleteSampleSubmission(string id)
	{
		_sampleRepository.Delete(id);
	}

	public void DeleteExpSubmission(string id)
	{
		_expRepository.Delete(id);
	}
}
