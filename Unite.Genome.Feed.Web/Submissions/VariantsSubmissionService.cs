using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Submissions;

public class VariantsSubmissionService
{
	private readonly Repositories.Variants.SsmSubmissionRepository _ssmRepository;
	private readonly Repositories.Variants.CnvSubmissionRepository _cnvRepository;
	private readonly Repositories.Variants.SvSubmissionRepository _svRepository;


    public VariantsSubmissionService(IMongoOptions options)
	{
		_ssmRepository = new Repositories.Variants.SsmSubmissionRepository(options);
		_cnvRepository = new Repositories.Variants.CnvSubmissionRepository(options);
		_svRepository = new Repositories.Variants.SvSubmissionRepository(options);
	}


	public string AddSsmSubmission(SequencingDataModel<Models.Variants.SSM.VariantModel> data)
	{
		return _ssmRepository.Add(data);
	}

    public string AddCnvSubmission(SequencingDataModel<Models.Variants.CNV.VariantModel> data)
    {
        return _cnvRepository.Add(data);
    }

    public string AddSvSubmission(SequencingDataModel<Models.Variants.SV.VariantModel> data)
    {
        return _svRepository.Add(data);
    }

	public SequencingDataModel<Models.Variants.SSM.VariantModel> FindSsmSubmission(string id)
	{
		return _ssmRepository.Find(id)?.Document;
	}

    public SequencingDataModel<Models.Variants.CNV.VariantModel> FindCnvSubmission(string id)
    {
        return _cnvRepository.Find(id)?.Document;
    }

    public SequencingDataModel<Models.Variants.SV.VariantModel> FindSvSubmission(string id)
    {
        return _svRepository.Find(id)?.Document;
    }

    public void DeleteSsmSubmission(string id)
    {
        _ssmRepository.Delete(id);
    }

    public void DeleteCnvSubmission(string id)
    {
        _cnvRepository.Delete(id);
    }

    public void DeleteSvSubmission(string id)
    {
        _svRepository.Delete(id);
    }
}
