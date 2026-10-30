namespace Unite.Omics.Feed.Data.Models;

public record SampleModel
{
    public string Genome;
    public double? Purity;
    public double? Ploidy;
    public int? Cells;
    public string Batch;

    public SpecimenModel Specimen;
    public AssayModel Assay;

    public IEnumerable<ResourceModel> Resources;
}
