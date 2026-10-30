using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public class SampleFormConverter : IFormConverter<SampleModel, SampleForm, EmptyModel>
{
    public virtual SampleModel Convert(SampleForm form, EmptyModel[] entries, ResourceModel[] resources)
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
            Batch = form.Batch,
            Resources = resources
        };
    }
}
