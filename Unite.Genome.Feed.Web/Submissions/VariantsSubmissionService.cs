using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Variants;

namespace Unite.Genome.Feed.Web.Submissions;

public class VariantsSubmissionService
{
	private readonly Repositories.Variants.SSM.DefaultSubmissionRepository _ssmDefaultRepository;
	private readonly Repositories.Variants.CNV.DefaultSubmissionRepository _cnvDefaultRepository;
	private readonly Repositories.Variants.CNV.AceSeqSubmissionRepository _cnvAceSeqRepository;
	private readonly Repositories.Variants.SV.DefaultSubmissionRepository _svDefaultRepository;


    public VariantsSubmissionService(IMongoOptions options)
	{
		_ssmDefaultRepository = new Repositories.Variants.SSM.DefaultSubmissionRepository(options);
		_cnvDefaultRepository = new Repositories.Variants.CNV.DefaultSubmissionRepository(options);
		_cnvAceSeqRepository = new Repositories.Variants.CNV.AceSeqSubmissionRepository(options);
		_svDefaultRepository = new Repositories.Variants.SV.DefaultSubmissionRepository(options);
	}


	public string AddSsmSubmission(SequencingDataModel<Models.Variants.SSM.VariantModel> data)
	{
		return _ssmDefaultRepository.Add(data);
	}

    public string AddCnvSubmission(SequencingDataModel<Models.Variants.CNV.VariantModel> data)
    {
        return _cnvDefaultRepository.Add(data);
    }

    public string AddCnvAceSeqSubmission(SequencingDataModel<Models.Variants.CNV.VariantAceSeqModel> data)
    {
        return _cnvAceSeqRepository.Add(data);
    }

    public string AddSvSubmission(SequencingDataModel<Models.Variants.SV.VariantModel> data)
    {
        return _svDefaultRepository.Add(data);
    }

	public SequencingDataModel<Models.Variants.SSM.VariantModel> FindSsmSubmission(string id)
	{
		return _ssmDefaultRepository.Find(id)?.Document;
	}

    public SequencingDataModel<Models.Variants.CNV.VariantModel> FindCnvSubmission(string id)
    {
        return _cnvDefaultRepository.Find(id)?.Document;
    }

    public SequencingDataModel<Models.Variants.CNV.VariantAceSeqModel> FindCnvAceSeqSubmission(string id)
    {
        return _cnvAceSeqRepository.Find(id)?.Document;
    }

    public SequencingDataModel<Models.Variants.SV.VariantModel> FindSvSubmission(string id)
    {
        return _svDefaultRepository.Find(id)?.Document;
    }

    public void DeleteSsmSubmission(string id)
    {
        _ssmDefaultRepository.Delete(id);
    }

    public void DeleteCnvSubmission(string id)
    {
        _cnvDefaultRepository.Delete(id);
    }

    public void DeleteCnvAceSeqSubmission(string id)
    {
        _cnvAceSeqRepository.Delete(id);
    }

    public void DeleteSvSubmission(string id)
    {
        _svDefaultRepository.Delete(id);
    }
}
