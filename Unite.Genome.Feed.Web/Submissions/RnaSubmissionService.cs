using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;
using Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

namespace Unite.Genome.Feed.Web.Submissions;

public class RnaSubmissionService
{
	private readonly ExpSubmissionRepository _expRepository;


	public RnaSubmissionService(IMongoOptions options)
	{
		_expRepository = new ExpSubmissionRepository(options);
	}


	public string AddExpSubmission(AnalysisModel<ExpressionModel> data)
	{
		return _expRepository.Add(data);
	}

	public AnalysisModel<ExpressionModel> FindExpSubmission(string id)
	{
		return _expRepository.Find(id)?.Document;
	}

	public void DeleteExpSubmission(string id)
	{
		_expRepository.Delete(id);
	}
}
