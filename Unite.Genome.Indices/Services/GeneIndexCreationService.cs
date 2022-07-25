using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Tissues.Enums;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Data.Services.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Services;

namespace Unite.Genome.Indices.Services;

public class GeneIndexCreationService : IIndexCreationService<GeneIndex>
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneIndexMapper _geneIndexMapper;
    private readonly MutationIndexMapper _mutationIndexMapper;
    private readonly DonorIndexMapper _donorIndexMapper;
    private readonly ImageIndexMapper _imageIndexMapper;
    private readonly SpecimenIndexMapper _specimenIndexMapper;


    public GeneIndexCreationService(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneIndexMapper = new GeneIndexMapper();
        _mutationIndexMapper = new MutationIndexMapper();
        _donorIndexMapper = new DonorIndexMapper();
        _imageIndexMapper = new ImageIndexMapper();
        _specimenIndexMapper = new SpecimenIndexMapper();
    }


    public GeneIndex CreateIndex(object key)
    {
        var geneId = (int)key;

        return CreateGeneIndex(geneId);
    }


    private GeneIndex CreateGeneIndex(int geneId)
    {
        var gene = LoadGene(geneId);

        if (gene == null)
        {
            return null;
        }

        var index = CreateGeneIndex(gene);

        return index;
    }

    private GeneIndex CreateGeneIndex(Gene gene)
    {
        var index = new GeneIndex();

        _geneIndexMapper.Map(gene, index);

        index.Mutations = CreateMutationIndices(gene.Id);

        index.NumberOfDonors = index.Mutations
            .SelectMany(mutation => mutation.Donors)
            .Select(donor => donor.Id)
            .Distinct()
            .Count();

        index.NumberOfMutations = index.Mutations
            .Select(mutation => mutation.Id)
            .Distinct()
            .Count();

        return index;
    }

    private Gene LoadGene(int geneId)
    {
        var gene = _dbContext.Genes
            .Include(gene => gene.Biotype)
            .Include(gene => gene.Info)
            .FirstOrDefault(gene => gene.Id == geneId);

        return gene;
    }


    private MutationIndex[] CreateMutationIndices(int geneId)
    {
        var mutations = LoadMutations(geneId);

        if (mutations == null)
        {
            return null;
        }

        var indices = mutations
            .Select(mutation => CreateMutationIndex(mutation, geneId))
            .ToArray();

        return indices;
    }

    private MutationIndex CreateMutationIndex(Mutation mutation, int geneId)
    {
        var index = new MutationIndex();

        _mutationIndexMapper.Map(mutation, index);

        index.Donors = CreateDonorIndices(mutation.Id);

        return index;
    }

    private Mutation[] LoadMutations(int geneId)
    {
        var mutationIds = _dbContext.AffectedTranscripts
            .Where(affectedTranscript => affectedTranscript.Transcript.GeneId == geneId)
            .Select(affectedTranscript => affectedTranscript.MutationId)
            .Distinct()
            .ToArray();

        var mutations = _dbContext.Mutations
            .IncludeAffectedTranscripts()
            .Where(mutation => mutationIds.Contains(mutation.Id))
            .ToArray();

        return mutations;
    }


    private DonorIndex[] CreateDonorIndices(long mutationId)
    {
        var donors = LoadDonors(mutationId);

        if (donors == null)
        {
            return null;
        }

        var indices = donors
            .Select(donor => CreateDonorindex(donor, mutationId))
            .ToArray();

        return indices;
    }

    private DonorIndex CreateDonorindex(Donor donor, long mutationId)
    {
        var index = new DonorIndex();

        var diagnosisDate = donor.ClinicalData?.DiagnosisDate;

        _donorIndexMapper.Map(donor, index);

        index.Specimens = CreateSpecimenIndices(donor.Id, mutationId, diagnosisDate);

        // Images can be asociated only with genes mutated in tumor tissues
        if (index.Specimens.Any(specimen => string.Equals(specimen.Tissue?.Type, TissueType.Tumor.ToDefinitionString())))
        {
            index.Images = CreateImageIndices(donor.Id, diagnosisDate);
        }

        return index;
    }

    private Donor[] LoadDonors(long mutationId)
    {
        var donorIds = _dbContext.MutationOccurrences
            .Where(mutationOccurrence => mutationOccurrence.MutationId == mutationId)
            .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId)
            .Distinct()
            .ToArray();

        var donors = _dbContext.Donors
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeWorkPackages()
            .IncludeStudies()
            .Where(donor => donorIds.Contains(donor.Id))
            .ToArray();

        return donors;
    }


    private ImageIndex[] CreateImageIndices(int donorId, DateOnly? diagnosisDate)
    {
        var images = LoadImages(donorId);

        if (images == null)
        {
            return null;
        }

        var indices = images
            .Select(image => CreateImageIndex(image, diagnosisDate))
            .ToArray();

        return indices;
    }

    private ImageIndex CreateImageIndex(Image image, DateOnly? diagnosisDate)
    {
        var index = new ImageIndex();

        _imageIndexMapper.Map(image, index, diagnosisDate);

        return index;
    }

    private Image[] LoadImages(int donorId)
    {
        var images = _dbContext.Set<Image>()
            .Include(image => image.MriImage)
            .Where(image => image.DonorId == donorId)
            .ToArray();

        return images;
    }


    private SpecimenIndex[] CreateSpecimenIndices(int donorId, long mutationId, DateOnly? diagnosisDate)
    {
        var specimens = LoadSpecimens(donorId, mutationId);

        if (specimens == null)
        {
            return null;
        }

        var indices = specimens
            .Select(specimen => CreateSpecimenIndex(specimen, diagnosisDate))
            .ToArray();

        return indices;
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen, DateOnly? diagnosisDate)
    {
        var index = new SpecimenIndex();

        _specimenIndexMapper.Map(specimen, index, diagnosisDate);

        return index;
    }

    private Specimen[] LoadSpecimens(int donorId, long mutationId)
    {
        var specimenIds = _dbContext.MutationOccurrences
            .Where(mutationOccurrence =>
                mutationOccurrence.MutationId == mutationId &&
                mutationOccurrence.AnalysedSample.Sample.Specimen.DonorId == donorId
            )
            .Select(mutationOccurrence => mutationOccurrence.AnalysedSample.Sample.SpecimenId)
            .Distinct()
            .ToArray();

        var specimens = _dbContext.Specimens
            .IncludeTissue()
            .IncludeCellLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeDrugScreeningData()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();

        return specimens;
    }
}
