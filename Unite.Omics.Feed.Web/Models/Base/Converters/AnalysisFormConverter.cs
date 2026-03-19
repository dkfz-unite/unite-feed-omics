using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Controllers;

public static class AnalysisFormConverter<TEntry> where TEntry : class, new()
{
    public static AnalysisModel<TEntry> Convert(AnalysisForm form)
    {
        return new AnalysisModel<TEntry>
        {
            TargetSample = ConvertSample(form),
            MatchedSample = ConvertMatchedSample(form)
        };
    }
    
    private static SampleModel ConvertSample(AnalysisForm form)
    {
        return new SampleModel
        {
            DonorId = form.DonorId,
            SpecimenId = form.SpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(form.SpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(form.AnalysisType).Value,
            AnalysisDate = form.AnalysisDate,
            AnalysisDay = form.AnalysisDay,
            Genome = form.Genome,
            Purity = form.Purity,
            Ploidy = form.Ploidy,
            Cells = form.Cells,
            Batch = form.Batch
        };
    }

    private static SampleModel ConvertMatchedSample(AnalysisForm form)
    {
        if (!HasMatchedSample(form))
            return null;

        return new SampleModel
        {
            DonorId = form.DonorId,
            SpecimenId = form.MatchedSpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(form.MatchedSpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(form.AnalysisType).Value,
            Genome = form.Genome
        };
    }
    
    private static bool HasMatchedSample(AnalysisForm form)
    {
        return !string.IsNullOrWhiteSpace(form.MatchedSpecimenId) &&
               !string.IsNullOrWhiteSpace(form.MatchedSpecimenType);
    }
}