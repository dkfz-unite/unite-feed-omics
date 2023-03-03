using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Submissions.Repositories.Transcriptomics;

namespace Unite.Genome.Feed.Web.Submissions;

public class TranscriptomicsSubmissionService
{
	private readonly DefaultSubmissionRepository _defaultRepository;


	public TranscriptomicsSubmissionService(IMongoOptions options)
	{
		_defaultRepository = new DefaultSubmissionRepository(options);
	}


	public string AddSubmission(TranscriptomicsDataModel data)
	{
		return _defaultRepository.Add(data);
	}

	public TranscriptomicsDataModel FindSubmission(string id)
	{
		return _defaultRepository.Find(id)?.Document;
	}

	public void DeleteSubmission(string id)
	{
		_defaultRepository.Delete(id);
	}
}
