using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Submissions;

public class DnaSubmissionService
{
    private readonly SampleSubmissionRepository _sampleRepository;
	private readonly SmSubmissionRepository _smRepository;
	private readonly CnvSubmissionRepository _cnvRepository;
	private readonly SvSubmissionRepository _svRepository;
	private readonly CnvProfileSubmissionRepository _cnvProfileRepository;
    
    public DnaSubmissionService(IMongoOptions options)
	{
        _cnvProfileRepository = new CnvProfileSubmissionRepository(options);
        _sampleRepository = new SampleSubmissionRepository(options);
		_smRepository = new SmSubmissionRepository(options);
		_cnvRepository = new CnvSubmissionRepository(options);
		_svRepository = new SvSubmissionRepository(options);
	}


    public string AddSampleSubmission(SampleModel data)
    {
        return _sampleRepository.Add(data);
    }

	public string AddSmSubmission(AnalysisModel<VariantModel> data)
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
    
    public string AddCnvProfileSubmission(AnalysisModel<CnvProfileModel> data)
    {
        return _cnvProfileRepository.Add(data);
    }

    public SampleModel FindSampleSubmission(string id)
    {
        return _sampleRepository.Find<SampleModel>(id)?.Document;
    }

	public AnalysisModel<VariantModel> FindSmSubmission(string id)
	{
		return _smRepository.Find<AnalysisModel<VariantModel>>(id)?.Document;
	}

    public AnalysisModel<Models.Dna.Cnv.VariantModel> FindCnvSubmission(string id)
    {
        return _cnvRepository.Find<AnalysisModel<Models.Dna.Cnv.VariantModel>>(id)?.Document;
    }

    public AnalysisModel<Models.Dna.Sv.VariantModel> FindSvSubmission(string id)
    {
        return _svRepository.Find<AnalysisModel<Models.Dna.Sv.VariantModel>>(id)?.Document;
    }

    public void DeleteSampleSubmission(string id)
    {
        _sampleRepository.Delete<SampleModel>(id);
    }

    public void DeleteSmSubmission(string id)
    {
        _smRepository.Delete<AnalysisModel<VariantModel>>(id);
    }

    public void DeleteCnvSubmission(string id)
    {
        _cnvRepository.Delete<AnalysisModel<Models.Dna.Cnv.VariantModel>>(id);
    }

    public void DeleteSvSubmission(string id)
    {
        _svRepository.Delete<AnalysisModel<Models.Dna.Sv.VariantModel>>(id);
    }
}
