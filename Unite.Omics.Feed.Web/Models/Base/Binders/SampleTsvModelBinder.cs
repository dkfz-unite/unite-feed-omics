using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv;

namespace Unite.Omics.Feed.Web.Models.Base.Binders;

public class SampleTsvModelBinder : IModelBinder
{
    protected const string _donor_id = "donor_id";
    protected const string _specimen_id = "specimen_id";
    protected const string _specimen_type = "specimen_type";
    protected const string _analysis_type = "analysis_type";
    protected const string _analysis_date = "analysis_date";
    protected const string _analysis_day = "analysis_day";
    protected const string _genome = "genome";
    protected const string _cells = "cells";
    protected const string _purity = "purity";
    protected const string _ploidy = "ploidy";


    public virtual async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);

        var map = CreateMap();

        var comments = new List<string>();
        
        var tsv = await reader.ReadToEndAsync();

        var model = new SampleModel()
        {
            Resources = TsvReader.Read(tsv, map, comments: comments).ToArray()
        };

        ParseComments(comments, ref model);

        bindingContext.Result = ModelBindingResult.Success(model);
    }


    private static ClassMap<ResourceModel> CreateMap()
    {
        return new ClassMap<ResourceModel>()
            .Map(entity => entity.Name, "name")
            .Map(entity => entity.Type, "type")
            .Map(entity => entity.Format, "format")
            .Map(entity => entity.Archive, "archive")
            .Map(entity => entity.Url, "url");
    }

    private static void ParseComments(List<string> comments, ref SampleModel model)
    {
        if (comments.IsEmpty())
            return;

        var comparison = StringComparison.InvariantCultureIgnoreCase;

        foreach (var comment in comments)
        {
            var parts = comment.Split(':').Select(part => part.Trim()).ToArray();

            if (parts.Length < 2)
                continue;

            if (parts[0].Equals(_donor_id, comparison))
                model.DonorId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_specimen_id))
                model.SpecimenId = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_specimen_type, comparison))
                model.SpecimenType = GetValue<SpecimenType?>(parts[1]);
            else if (parts[0].Equals(_analysis_type, comparison))
                model.AnalysisType = GetValue<AnalysisType?>(parts[1]);
            else if (parts[0].Equals(_analysis_date, comparison))
                model.AnalysisDate = GetValue<DateOnly?>(parts[1]);
            else if (parts[0].Equals(_analysis_day, comparison))
                model.AnalysisDay = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_genome, comparison))
                model.Genome = GetValue<string>(parts[1]);
            else if (parts[0].Equals(_cells, comparison))
                model.Cells = GetValue<int?>(parts[1]);
            else if (parts[0].Equals(_purity, comparison))
                model.Purity = GetValue<double?>(parts[1]);
            else if (parts[0].Equals(_ploidy, comparison))
                model.Ploidy = GetValue<double?>(parts[1]);
        }
    }

    private static T GetValue<T>(string value)
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

    private static T ToEnum<T>(string value)
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
