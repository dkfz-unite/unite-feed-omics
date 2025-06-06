using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions;

public class DnaSubmissionService
{
    private readonly Repositories.Dna.SampleSubmissionRepository _sampleRepository;
	private readonly Repositories.Dna.SmSubmissionRepository _smRepository;
	private readonly Repositories.Dna.CnvSubmissionRepository _cnvRepository;
	private readonly Repositories.Dna.SvSubmissionRepository _svRepository;


    public DnaSubmissionService(IMongoOptions options)
	{
        _sampleRepository = new Repositories.Dna.SampleSubmissionRepository(options);
		_smRepository = new Repositories.Dna.SmSubmissionRepository(options);
		_cnvRepository = new Repositories.Dna.CnvSubmissionRepository(options);
		_svRepository = new Repositories.Dna.SvSubmissionRepository(options);
	}


    public string AddSampleSubmission(SampleModel data)
    {
        return _sampleRepository.Add(data);
    }

	public string AddSmSubmission(AnalysisModel<Models.Dna.Sm.VariantModel> data)
	{
		return _smRepository.Add(data);
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

	public AnalysisModel<Models.Dna.Sm.VariantModel> FindSmSubmission(string id)
	{
		return _smRepository.Find(id)?.Document;
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

    public void DeleteSmSubmission(string id)
    {
        _smRepository.Delete(id);
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
