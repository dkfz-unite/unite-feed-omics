using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;
using Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

namespace Unite.Genome.Feed.Web.Submissions;

public class RnaSubmissionService
{
	private readonly SampleSubmissionRepository _sampleRepository;
	private readonly ExpSubmissionRepository _expRepository;


	public RnaSubmissionService(IMongoOptions options)
	{
		_sampleRepository = new SampleSubmissionRepository(options);
		_expRepository = new ExpSubmissionRepository(options);
	}


	public string AddSampleSubmission(SampleModel data)
	{
		return _sampleRepository.Add(data);
	}

	public string AddExpSubmission(AnalysisModel<ExpressionModel> data)
	{
		return _expRepository.Add(data);
	}

	public SampleModel FindSampleSubmission(string id)
	{
		return _sampleRepository.Find(id)?.Document;
	}

	public AnalysisModel<ExpressionModel> FindExpSubmission(string id)
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
