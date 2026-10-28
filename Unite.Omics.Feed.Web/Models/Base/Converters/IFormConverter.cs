namespace Unite.Omics.Feed.Web.Models.Base.Converters;

public interface IFormConverter<TModel, TForm, TEntry>
    where TModel : SubmissionModel
    where TForm : SubmissionForm
    where TEntry : class
{
    TModel Convert(TForm form, TEntry[] entries, ResourceModel[] resources);
}
