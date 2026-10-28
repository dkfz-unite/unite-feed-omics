using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

/// <summary>
/// Converts an AnalysisForm into an AnalysisModel with the specified entry type.
/// </summary>
/// <typeparam name="TEntry">Entry type.</typeparam>
public class AnalysisFormConverter<TEntry> : IFormConverter<AnalysisModel<TEntry>, AnalysisForm, TEntry> where TEntry : class, new()
{
    public virtual AnalysisModel<TEntry> Convert(AnalysisForm form, TEntry[] entries, ResourceModel[] resources)
    {
        return new AnalysisModel<TEntry>
        {
            TargetSample = ConvertSample(form),
            MatchedSample = ConvertMatchedSample(form),
            Entries = entries,
            Resources = resources
        };
    }


    protected virtual SampleModel ConvertSample(AnalysisForm form)
    {
        return new SampleModel
        {
            DonorId = form.DonorId,
            SpecimenId = form.SpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(form.SpecimenType),
            AnalysisType = EnumBinder.Bind<AnalysisType>(form.AnalysisType),
            Scope = form.Scope,
            AnalysisDate = form.AnalysisDate,
            AnalysisDay = form.AnalysisDay,
            Genome = form.Genome,
            Purity = form.Purity,
            Ploidy = form.Ploidy,
            Cells = form.Cells,
            Batch = form.Batch
        };
    }

    protected virtual SampleModel ConvertMatchedSample(AnalysisForm form)
    {
        if (!HasMatchedSample(form))
            return null;

        return new SampleModel
        {
            DonorId = form.DonorId,
            SpecimenId = form.MatchedSpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(form.MatchedSpecimenType),
            AnalysisType = EnumBinder.Bind<AnalysisType>(form.AnalysisType),
            Genome = form.Genome
        };
    }


    private static bool HasMatchedSample(AnalysisForm form)
    {
        return !string.IsNullOrWhiteSpace(form.MatchedSpecimenId) ||
               !string.IsNullOrWhiteSpace(form.MatchedSpecimenType);
    }
}
