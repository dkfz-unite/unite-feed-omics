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
            EntriesScope = form.EntriesScope?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
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
            AssayType = EnumBinder.Bind<AnalysisType>(form.AssayType),
            AssayDate = form.AssayDate,
            AssayDay = form.AssayDay,
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
            AssayType = EnumBinder.Bind<AnalysisType>(form.AssayType),
            Genome = form.Genome
        };
    }


    private static bool HasMatchedSample(AnalysisForm form)
    {
        return !string.IsNullOrWhiteSpace(form.MatchedSpecimenId) ||
               !string.IsNullOrWhiteSpace(form.MatchedSpecimenType);
    }
}
