using Microsoft.EntityFrameworkCore;
using Unite.Essentials.Extensions;
using Unite.Data.Entities.Specimens;
using Unite.Indices.Entities.Variants;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Genome.Indices.Services;

public abstract class VariantIndexCreator<TVariant, TVariantEntry>
    where TVariant : Data.Entities.Genome.Analysis.Dna.Variant
    where TVariantEntry : Data.Entities.Genome.Analysis.Dna.VariantEntry<TVariant>
{
    protected readonly VariantIndexingCache<TVariant, TVariantEntry> _cache;

    public VariantIndexCreator(VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        _cache = cache;
    }


    protected SpecimenIndex[] CreateSpecimenIndices(int variantId)
    {
        var specimens = LoadSpecimens(variantId);

        return specimens.Select(CreateSpecimenIndex).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);

        return index;
    }

    private Specimen[] LoadSpecimens(int variantId)
    {
        var sampleIds = _cache.Entries
            .Where(entry => entry.EntityId == variantId)
            .Select(entry => entry.SampleId)
            .Distinct()
            .ToArray();

        var specimenIds = _cache.Samples
            .Where(sample => sampleIds.Contains(sample.Id))
            .Select(sample => sample.SpecimenId)
            .Distinct()
            .ToArray();

        return _cache.Specimens
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }

    
    protected StatsIndex CreateStatsIndex(int variantId)
    {
        var variant = _cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
        var sampleIds = _cache.Entries.Where(entry => entry.EntityId == variantId).Select(entry => entry.SampleId).Distinct().ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();

        var geneIds = GetAffectedGenes(variant);

        return new StatsIndex
        {
            Donors = donorIds.Length,
            Genes = geneIds?.Length ?? 0,
        };
    }

    protected abstract int[] GetAffectedGenes(TVariant variant);


    protected DataIndex CreateDataIndex(int variantId)
    {
        var variant = _cache.Variants.First(variant => variant.Id == variantId);
        var sampleIds = _cache.Entries.Where(entry => entry.EntityId == variantId).Select(entry => entry.SampleId).Distinct().ToArray();
        var specimenIds = _cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = _cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();

        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorIds),
            Treatments = CheckTreatments(donorIds),
            Mris = CheckImages(donorIds, ImageType.MRI),
            Cts = CheckImages(donorIds, ImageType.CT),
            Materials = CheckSpecimens(specimenIds, SpecimenType.Material),
            MaterialsMolecular = CheckMolecularData(specimenIds, SpecimenType.Material),
            Lines = CheckSpecimens(specimenIds, SpecimenType.Line),
            LinesMolecular = CheckMolecularData(specimenIds, SpecimenType.Line),
            LinesInterventions = CheckInterventions(specimenIds, SpecimenType.Line),
            LinesDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Line),
            Organoids = CheckSpecimens(specimenIds, SpecimenType.Organoid),
            OrganoidsMolecular = CheckMolecularData(specimenIds, SpecimenType.Organoid),
            OrganoidsInterventions = CheckInterventions(specimenIds, SpecimenType.Organoid),
            OrganoidsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Organoid),
            Xenografts = CheckSpecimens(specimenIds, SpecimenType.Xenograft),
            XenograftsMolecular = CheckMolecularData(specimenIds, SpecimenType.Xenograft),
            XenograftsInterventions = CheckInterventions(specimenIds, SpecimenType.Xenograft),
            XenograftsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Xenograft),
            Ssms = variant is Data.Entities.Genome.Analysis.Dna.Ssm.Variant,
            Cnvs = variant is Data.Entities.Genome.Analysis.Dna.Cnv.Variant,
            Svs = variant is Data.Entities.Genome.Analysis.Dna.Sv.Variant,
            Meth = CheckMethylation(sampleIds),
            Exp = CheckGeneExp(sampleIds),
            ExpSc = CheckGeneExpScores(sampleIds)
        };
    }

    private bool CheckClinicalData(int[] donorIds)
    {
        return _cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.ClinicalData != null
        );
    }

    private bool CheckTreatments(int[] donorIds)
    {
        return _cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.Treatments?.Any() == true
        );
    }

    private bool CheckImages(int[] donorIds, ImageType type)
    {
        return _cache.Images.Any(image => 
            donorIds.Contains(image.DonorId) && 
            image.TypeId == type
        );
    }

    private bool CheckSpecimens(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type
        );
    }

    private bool CheckMolecularData(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.MolecularData != null
        );
    }

    private bool CheckInterventions(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.Interventions?.Any() == true
        );
    }

    private bool CheckDrugScreenings(int[] specimenIds, SpecimenType type)
    {
        return _cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.SpecimenSamples?.Any(sample => sample.DrugScreenings?.Any() == true) == true
        );
    }

    private bool CheckMethylation(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == "dna-meth") == true
        );
    }

    private bool CheckGeneExp(int[] sampleIds)
    {
        return _cache.Expressions.Any(expression => 
            sampleIds.Contains(expression.SampleId)
        );
    }

    private bool CheckGeneExpScores(int[] sampleIds)
    {
        return _cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == "rnasc-exp") == true
        );
    }
}
