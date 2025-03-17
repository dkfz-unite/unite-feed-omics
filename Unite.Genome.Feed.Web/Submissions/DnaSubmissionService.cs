using Unite.Cache.Configuration.Options;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Submissions;

public class DnaSubmissionService
{
    private readonly Repositories.Dna.SampleSubmissionRepository _sampleRepository;
	private readonly Repositories.Dna.SsmSubmissionRepository _ssmRepository;
	private readonly Repositories.Dna.CnvSubmissionRepository _cnvRepository;
	private readonly Repositories.Dna.SvSubmissionRepository _svRepository;


    public DnaSubmissionService(IMongoOptions options)
	{
        _sampleRepository = new Repositories.Dna.SampleSubmissionRepository(options);
		_ssmRepository = new Repositories.Dna.SsmSubmissionRepository(options);
		_cnvRepository = new Repositories.Dna.CnvSubmissionRepository(options);
		_svRepository = new Repositories.Dna.SvSubmissionRepository(options);
	}


    public string AddSampleSubmission(SampleModel data)
    {
        return _sampleRepository.Add(data);
    }

	public string AddSsmSubmission(AnalysisModel<Models.Dna.Ssm.VariantModel> data)
	{
		return _ssmRepository.Add(data);
	}

    public string AddCnvSubmission(AnalysisModel<Models.Dna.Cnv.VariantModel> data)
    {
        return _cnvRepository.Add(data);
    }

    public string AddSvSubmission(AnalysisModel<Models.Dna.Sv.VariantModel> data)
    {
        return _svRepository.Add(data);
    }

    public SampleModel FindSampleSubmission(string id)
    {
        return _sampleRepository.Find(id)?.Document;
    }

	public AnalysisModel<Models.Dna.Ssm.VariantModel> FindSsmSubmission(string id)
	{
		return _ssmRepository.Find(id)?.Document;
	}

    public AnalysisModel<Models.Dna.Cnv.VariantModel> FindCnvSubmission(string id)
    {
        return _cnvRepository.Find(id)?.Document;
    }

    public AnalysisModel<Models.Dna.Sv.VariantModel> FindSvSubmission(string id)
    {
        return _svRepository.Find(id)?.Document;
    }

    public void DeleteSampleSubmission(string id)
    {
        _sampleRepository.Delete(id);
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
