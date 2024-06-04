using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;

namespace Unite.Genome.Feed.Web.Models.Base.Binders;

public abstract class AnalysisTsvModelBinder<TModel> : IModelBinder
    where TModel : class, new()
{
    protected const string _tsample_donor_id = "tsample_donor_id";
    protected const string _tsample_specimen_id = "tsample_specimen_id";
    protected const string _tsample_specimen_type = "tsample_specimen_type";
    protected const string _tsample_purity = "tsample_purity";
    protected const string _tsample_ploidy = "tsample_ploidy";
    protected const string _tsample_cells_number = "tsample_cells_number";
    protected const string _tsample_genes_model = "tsample_genes_model";
    protected const string _tsample_analysis_type = "tsample_analysis_type";
    protected const string _tsample_analysis_date = "tsample_analysis_date";
    protected const string _tsample_analysis_day = "tsample_analysis_day";
    protected const string _tsample_resource_type = "tsample_resource_type";
    protected const string _tsample_resource_format = "tsample_resource_format";
    protected const string _tsample_resource_url = "tsample_resource_url";

    protected const string _msample_donor_id = "msample_donor_id";
    protected const string _msample_specimen_id = "msample_specimen_id";
    protected const string _msample_specimen_type = "msample_specimen_type";
    protected const string _msample_analysis_type = "msample_analysis_type";
    protected const string _msample_analysis_date = "msample_analysis_date";
    protected const string _msample_analysis_day = "msample_analysis_day";
    protected const string _msample_resource_type = "msample_resource_type";
    protected const string _msample_resource_format = "msample_resource_format";
    protected const string _msample_resource_url = "msample_resource_url";


    public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var map = CreateMap();

        var comments = new List<string>();
        
        var tsv = await reader.ReadToEndAsync();

        var model = new AnalysisModel<TModel>()
        {
            Entries = TsvReader.Read(tsv, map, comments: comments).ToArray()
        };

        ParseComments(comments, ref model);

        bindingContext.Result = ModelBindingResult.Success(model);
    }


    protected abstract ClassMap<TModel> CreateMap();

    protected virtual void ParseComments(List<string> comments, ref AnalysisModel<TModel> model)
    {
        if (comments.IsEmpty())
            return;

        var comparison = StringComparison.InvariantCultureIgnoreCase;

        var targetSample = new SampleModel();
        var targetResource = new ResourceModel();
        var matchedSample = new SampleModel();
        var matchedResource = new ResourceModel();

        foreach (var comment in comments)
        {
            var parts = comment.Split(':').Select(part => part.Trim()).ToArray();

            if (parts.Length < 2)
                continue;

            // Target sample
            if (parts[0].Equals(_tsample_donor_id, comparison))
                targetSample.DonorId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_specimen_id))
                targetSample.SpecimenId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_specimen_type, comparison))
                targetSample.SpecimenType = GetValue<SpecimenType?>(parts[1]);
            else if (parts[0].Equals(_tsample_purity, comparison))
                targetSample.Purity = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_tsample_ploidy, comparison))
                targetSample.Ploidy = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_tsample_cells_number, comparison))
                targetSample.CellsNumber = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_tsample_genes_model, comparison))
                targetSample.GenesModel = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_analysis_type, comparison))
                targetSample.AnalysisType = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_tsample_analysis_date, comparison))
                targetSample.AnalysisDate = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_tsample_analysis_day, comparison))
                targetSample.AnalysisDay = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_tsample_resource_type, comparison))
                targetResource.Type = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_resource_format, comparison))
                targetResource.Format = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_resource_url, comparison))
                targetResource.Url = GetValue<string>(parts[1]);

            // Matched sample
            else if (parts[0].Equals(_msample_specimen_id, comparison))
                matchedSample.SpecimenId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_msample_specimen_type, comparison))
                matchedSample.SpecimenType = GetValue<SpecimenType?>(parts[1]);
            else if (parts[0].Equals(_msample_analysis_type, comparison))
                matchedSample.AnalysisType = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_msample_analysis_date, comparison))
                matchedSample.AnalysisDate = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_msample_analysis_day, comparison))
                matchedSample.AnalysisDay = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_msample_resource_type, comparison))
                matchedResource.Type = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_msample_resource_format, comparison))
                matchedResource.Format = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_msample_resource_url, comparison))
                matchedResource.Url = GetValue<string>(parts[1]);
        }
        
        if (IsNotEmpty(targetSample))
            model.TargetSample = targetSample;
        
        if (IsNotEmpty(targetResource))
            model.TargetSample.Resources = [targetResource];

        if (IsNotEmpty(matchedSample))
            model.MatchedSample = matchedSample;

        if (IsNotEmpty(matchedResource))
            model.MatchedSample.Resources = [matchedResource];
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

    protected bool IsNotEmpty(SampleModel model)
    {
        var properties = model.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(model);

            if (value != null)
                return true;
        }

        return false;
    }

    protected bool IsNotEmpty(ResourceModel model)
    {
        var properties = model.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(model);

            if (value != null)
                return true;
        }

        return false;
    }
}
