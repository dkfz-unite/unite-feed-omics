using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;

namespace Unite.Genome.Feed.Web.Models.Base.Binders;

public abstract class SequencingDataTsvModelBinder<TModel> : IModelBinder
    where TModel : class, new()
{
    protected const string _analysis_id = "analysis_id";
    protected const string _analysis_type = "analysis_type";
    protected const string _analysis_date = "analysis_date";
    protected const string _analysis_day = "analysis_day";
    protected const string _donor_id = "donor_id";
    protected const string _target_sample_id = "target_sample_id";
    protected const string _target_sample_type = "target_sample_type";
    protected const string _target_sample_ploidy = "target_sample_ploidy";
    protected const string _target_sample_purity = "target_sample_purity";
    protected const string _target_sample_cells_number = "target_sample_cells_number";
    protected const string _target_sample_genes_model = "target_sample_genes_model"; 
    protected const string _matched_sample_id = "matched_sample_id";
    protected const string _matched_sample_type = "matched_sample_type";
    protected const string _resource_type = "resource_type";
    protected const string _resource_path = "resource_path";
    protected const string _resource_url = "resource_url";


    public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var map = CreateMap();

        var comments = new List<string>();
        
        var tsv = await reader.ReadToEndAsync();

        var model = new SequencingDataModel<TModel>()
        {
            Entries = TsvReader.Read(tsv, map, comments: comments).ToArray()
        };

        ParseComments(comments, ref model);

        bindingContext.Result = ModelBindingResult.Success(model);
    }


    protected abstract ClassMap<TModel> CreateMap();

    protected virtual void ParseComments(List<string> comments, ref SequencingDataModel<TModel> model)
    {
        if (comments.IsEmpty())
            return;

        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var analysis = new AnalysisModel();
        var targetSample = new SampleModel();
        var matchedSample = new SampleModel();
        var resource = new ResourceModel();

        foreach (var comment in comments)
        {
            var parts = comment.Split(':').Select(part => part.Trim()).ToArray();

            if (parts.Length < 2)
                continue;

            if (parts[0].Equals(_analysis_id, comparison))
                analysis.Id = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_analysis_type, comparison))
                analysis.Type = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_analysis_date, comparison))
                analysis.Date = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_analysis_day, comparison))
                analysis.Day = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_donor_id, comparison))
                targetSample.DonorId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_target_sample_id, comparison))
                targetSample.SpecimenId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_target_sample_type, comparison))
                targetSample.SpecimenType = GetValue<SpecimenType?>(parts[1]);
            else if (parts[0].Equals(_target_sample_ploidy, comparison))
                targetSample.Ploidy = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_target_sample_purity, comparison))
                targetSample.Purity = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_target_sample_cells_number, comparison))
                targetSample.CellsNumber = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_target_sample_genes_model, comparison))
                targetSample.GenesModel = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_matched_sample_id, comparison))
                matchedSample.SpecimenId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_matched_sample_type, comparison))
                matchedSample.SpecimenType = GetValue<SpecimenType?>(parts[1]);
            else if (parts[0].Equals(_resource_type, comparison))
                resource.Type = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_resource_path, comparison))
                resource.Path = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_resource_url, comparison))
                resource.Url = GetValue<string>(parts[1]);
        }

        if (analysis.IsNotEmpty())
            model.Analysis = analysis;
        
        if (targetSample.IsNotEmpty())
            model.TargetSample = targetSample;

        if (matchedSample.IsNotEmpty())
            model.MatchedSample = matchedSample;

        if (resource.IsNotEmpty())
            model.Resources = [resource];

        if (model.MatchedSample != null)
            model.MatchedSample.DonorId = model.TargetSample.DonorId;
    }

    protected virtual T GetValue<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;
        else if (typeof(T) == typeof(string))
            return (T)(object)value;
        else if (typeof(T) == typeof(int?))
            return (T)(object)int.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        else if (typeof(T) == typeof(double?))
            return (T)(object)double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        else if (typeof(T) == typeof(DateOnly?))
            return (T)(object)DateOnly.Parse(value, CultureInfo.InvariantCulture);
        else if (typeof(T) == typeof(AnalysisType?))
            return (T)(object)Enum.Parse<AnalysisType>(value);
        else if (typeof(T) == typeof(SpecimenType?))
            return (T)(object)Enum.Parse<SpecimenType>(value);
        else
            return (T)Convert.ChangeType(value, typeof(T));
    }
}
