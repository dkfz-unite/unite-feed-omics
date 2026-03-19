using Unite.Essentials.Tsv.Attributes;
using Unite.Essentials.Tsv.Converters;

namespace Unite.Omics.Feed.Web.Models.Prot.Readers.DiaNN;

public class Expression
{
    private string[] _accessions;
    private double _intensity;


    [Column(name: "Protein.Group", converterType: typeof(ArrayConverter))]
    public string[] Accessions { get => _accessions; set => _accessions = value; }

    [Column(name: "Quantity")]
    public double Intensity { get => _intensity; set => _intensity = value; }
}

internal class ArrayConverter : IConverter<string[]>
{
    public object Convert(string value, string row)
    {
        return value?.Trim('"').Split([',',';',' '], StringSplitOptions.TrimEntries).ToArray();
    }

    public string Convert(object value, object row)
    {
        throw new NotImplementedException();
    }
}
