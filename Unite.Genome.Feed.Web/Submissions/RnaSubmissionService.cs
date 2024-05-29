using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;
using Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

namespace Unite.Genome.Feed.Web.Submissions;

public class RnaSubmissionService
{
	private readonly BulkSubmissionRepository _bulkRepository;
	private readonly CellSubmissionRepository _cellRepository;


	public RnaSubmissionService(IMongoOptions options)
	{
		_bulkRepository = new BulkSubmissionRepository(options);
		_cellRepository = new CellSubmissionRepository(options);
	}


	public string AddBulkSubmission(SeqDataModel<BulkExpressionModel> data)
	{
		return _bulkRepository.Add(data);
	}

	public string AddCellSubmission(SeqDataModel<CellExpressionModel> data)
	{
		return _cellRepository.Add(data);
	}

	public SeqDataModel<BulkExpressionModel> FindBulkSubmission(string id)
	{
		return _bulkRepository.Find(id)?.Document;
	}

	public SeqDataModel<CellExpressionModel> FindCellSubmission(string id)
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
