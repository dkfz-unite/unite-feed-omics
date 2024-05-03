using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Submissions.Repositories.Transcriptomics;

namespace Unite.Genome.Feed.Web.Submissions;

public class TranscriptomicsSubmissionService
{
	private readonly BulkSubmissionRepository _bulkRepository;
	private readonly CellSubmissionRepository _cellRepository;


	public TranscriptomicsSubmissionService(IMongoOptions options)
	{
		_bulkRepository = new BulkSubmissionRepository(options);
		_cellRepository = new CellSubmissionRepository(options);
	}


	public string AddBulkSubmission(SequencingDataModel<BulkExpressionModel> data)
	{
		return _bulkRepository.Add(data);
	}

	public string AddCellSubmission(SequencingDataModel<CellExpressionModel> data)
	{
		return _cellRepository.Add(data);
	}

	public SequencingDataModel<BulkExpressionModel> FindBulkSubmission(string id)
	{
		return _bulkRepository.Find(id)?.Document;
	}

	public SequencingDataModel<CellExpressionModel> FindCellSubmission(string id)
	{
		return _cellRepository.Find(id)?.Document;
	}

	public void DeleteBulkSubmission(string id)
	{
		_bulkRepository.Delete(id);
	}

	public void DeleteCellSubmission(string id)
	{
		_cellRepository.Delete(id);
	}
}
