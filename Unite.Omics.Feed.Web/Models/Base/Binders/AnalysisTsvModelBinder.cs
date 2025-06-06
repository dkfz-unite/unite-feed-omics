using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;

namespace Unite.Omics.Feed.Web.Models.Base.Binders;

public abstract class AnalysisTsvModelBinder<TModel> : IModelBinder
    where TModel : class, new()
{
    protected const string _tsample_donor_id = "tsample_donor_id";
    protected const string _tsample_specimen_id = "tsample_specimen_id";
    protected const string _tsample_specimen_type = "tsample_specimen_type";
    protected const string _tsample_analysis_type = "tsample_analysis_type";
    protected const string _tsample_analysis_date = "tsample_analysis_date";
    protected const string _tsample_analysis_day = "tsample_analysis_day";
    protected const string _tsample_genome = "tsample_genome";
    protected const string _tsample_purity = "tsample_purity";
    protected const string _tsample_ploidy = "tsample_ploidy";
    protected const string _tsample_cells = "tsample_cells";

    protected const string _msample_donor_id = "msample_donor_id";
    protected const string _msample_specimen_id = "msample_specimen_id";
    protected const string _msample_specimen_type = "msample_specimen_type";
    protected const string _msample_analysis_type = "msample_analysis_type";
    protected const string _msample_analysis_date = "msample_analysis_date";
    protected const string _msample_analysis_day = "msample_analysis_day";
    protected const string _msample_genome = "msample_genome";


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
        var matchedSample = new SampleModel();

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
            else if (parts[0].Equals(_tsample_analysis_type, comparison))
                targetSample.AnalysisType = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_tsample_analysis_date, comparison))
                targetSample.AnalysisDate = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_tsample_analysis_day, comparison))
                targetSample.AnalysisDay = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_tsample_genome, comparison))
                targetSample.Genome = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_tsample_purity, comparison))
                targetSample.Purity = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_tsample_ploidy, comparison))
                targetSample.Ploidy = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_tsample_cells, comparison))
                targetSample.Cells = GetValue<int?>(parts[1]);

            // Matched sample
            else if (parts[0].Equals(_msample_donor_id, comparison))
                matchedSample.DonorId = GetValue<string>(parts[1]);
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
            else if (parts[0].Equals(_msample_genome, comparison))
                matchedSample.Genome = GetValue<string>(parts[1]);
        }
        
        if (IsNotEmpty(targetSample))
            model.TargetSample = targetSample;

        if (IsNotEmpty(matchedSample))
            model.MatchedSample = matchedSample;
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
            return (T)(object)ToEnum<AnalysisType>(value);
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

    public static T ToEnum<T>(string value)
    {   
        var type = typeof(T);

        foreach (var name in Enum.GetNames(type))
        {
            var attribute = ((EnumMemberAttribute[])type.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            
            if (attribute.Value == value)
                return (T)Enum.Parse(type, name);
        }

        return default;
    }
}
