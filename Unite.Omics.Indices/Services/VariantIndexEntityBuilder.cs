using Unite.Essentials.Extensions;
using Unite.Data.Entities.Specimens;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services.Mappers;
using Unite.Indices.Entities;
using Unite.Data.Entities.Images.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Data.Constants;

namespace Unite.Omics.Indices.Services;

public abstract class VariantIndexEntityBuilder<TVariant, TVariantEntry, TIndexEntity> : IndexEntityBuilder<TIndexEntity, VariantIndexingCache<TVariant, TVariantEntry>>
    where TVariant : Data.Entities.Omics.Analysis.Dna.Variant
    where TVariantEntry : Data.Entities.Omics.Analysis.Dna.VariantEntry<TVariant>
{
    protected SpecimenIndex[] CreateSpecimenIndices(int variantId, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        var specimens = LoadSpecimens(variantId, cache);

        return specimens.Select(CreateSpecimenIndex).ToArrayOrNull();
    }

    private SpecimenIndex CreateSpecimenIndex(Specimen specimen)
    {
        var index = SpecimenIndexMapper.CreateFrom<SpecimenIndex>(specimen, null);

        return index;
    }

    private Specimen[] LoadSpecimens(int variantId, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        var sampleIds = cache.Entries
            .Where(entry => entry.EntityId == variantId)
            .Select(entry => entry.SampleId)
            .Distinct()
            .ToArray();

        var specimenIds = cache.Samples
            .Where(sample => sampleIds.Contains(sample.Id))
            .Select(sample => sample.SpecimenId)
            .Distinct()
            .ToArray();

        return cache.Specimens
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArray();
    }

    
    protected StatsIndex CreateStatsIndex(int variantId, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        var variant = cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
        var sampleIds = cache.Entries.Where(entry => entry.EntityId == variantId).Select(entry => entry.SampleId).Distinct().ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();

        var geneIds = GetAffectedGenes(variant);

        return new StatsIndex
        {
            Donors = donorIds.Length,
            Genes = geneIds?.Length ?? 0,
        };
    }

    protected abstract int[] GetAffectedGenes(TVariant variant);


    protected DataIndex CreateDataIndex(int variantId, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        var variant = cache.Variants.First(variant => variant.Id == variantId);
        var sampleIds = cache.Entries.Where(entry => entry.EntityId == variantId).Select(entry => entry.SampleId).Distinct().ToArray();
        var specimenIds = cache.Samples.Where(sample => sampleIds.Contains(sample.Id)).Select(sample => sample.SpecimenId).Distinct().ToArray();
        var donorIds = cache.Specimens.Where(specimen => specimenIds.Contains(specimen.Id)).Select(specimen => specimen.DonorId).Distinct().ToArray();

        return new DataIndex
        {
            Donors = true,
            Clinical = CheckClinicalData(donorIds, cache),
            Treatments = CheckTreatments(donorIds, cache),
            Mrs = CheckImages(donorIds, ImageType.MR, cache),
            Cts = CheckImages(donorIds, ImageType.CT, cache),
            Materials = CheckSpecimens(specimenIds, SpecimenType.Material, cache),
            MaterialsMolecular = CheckMolecularData(specimenIds, SpecimenType.Material, cache),
            Lines = CheckSpecimens(specimenIds, SpecimenType.Line, cache),
            LinesMolecular = CheckMolecularData(specimenIds, SpecimenType.Line, cache),
            LinesInterventions = CheckInterventions(specimenIds, SpecimenType.Line, cache),
            LinesDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Line, cache),
            Organoids = CheckSpecimens(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsMolecular = CheckMolecularData(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsInterventions = CheckInterventions(specimenIds, SpecimenType.Organoid, cache),
            OrganoidsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Organoid, cache),
            Xenografts = CheckSpecimens(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsMolecular = CheckMolecularData(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsInterventions = CheckInterventions(specimenIds, SpecimenType.Xenograft, cache),
            XenograftsDrugs = CheckDrugScreenings(specimenIds, SpecimenType.Xenograft, cache),
            Sms = variant is Data.Entities.Omics.Analysis.Dna.Sm.Variant,
            Cnvs = variant is Data.Entities.Omics.Analysis.Dna.Cnv.Variant,
            Svs = variant is Data.Entities.Omics.Analysis.Dna.Sv.Variant,
            Meth = CheckMethylation(sampleIds, cache),
            Exp = CheckGeneExp(sampleIds, cache),
            ExpSc = CheckGeneExpSc(sampleIds, cache),
            Prot = CheckProtExp(sampleIds, cache)
        };
    }
    
    protected TVariant[] LoadSimilarVariants(int variantId, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        if (cache.SimilarVariants.TryGetValue(variantId, out var value))
            return value;

        return [];
    }

    private bool CheckClinicalData(int[] donorIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.ClinicalData != null
        );
    }

    private bool CheckTreatments(int[] donorIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Donors.Any(donor => 
            donorIds.Contains(donor.Id) && 
            donor.Treatments?.Any() == true
        );
    }

    private bool CheckImages(int[] donorIds, ImageType type, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Images.Any(image => 
            donorIds.Contains(image.DonorId) && 
            image.TypeId == type
        );
    }

    private bool CheckSpecimens(int[] specimenIds, SpecimenType type, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type
        );
    }

    private bool CheckMolecularData(int[] specimenIds, SpecimenType type, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.MolecularData != null
        );
    }

    private bool CheckInterventions(int[] specimenIds, SpecimenType type, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.Interventions?.Any() == true
        );
    }

    private bool CheckDrugScreenings(int[] specimenIds, SpecimenType type, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Specimens.Any(specimen => 
            specimenIds.Contains(specimen.Id) && 
            specimen.TypeId == type && 
            specimen.SpecimenSamples?.Any(sample => sample.DrugScreenings?.Any() == true) == true
        );
    }

    private bool CheckMethylation(int[] sampleIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource =>
                (resource.Type == DataTypes.Omics.Methylation.Sample && resource.Format == FileTypes.Sequence.Idat) ||
                (resource.Type == DataTypes.Omics.Methylation.Level)) == true
        );
    }

    private bool CheckGeneExp(int[] sampleIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.GeneExpressions.Any(expression => 
            sampleIds.Contains(expression.SampleId)
        );
    }

    private bool CheckGeneExpSc(int[] sampleIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Expression) == true
        );
    }

    private bool CheckProtExp(int[] sampleIds, VariantIndexingCache<TVariant, TVariantEntry> cache)
    {
        return cache.Samples.Any(sample => 
            sampleIds.Contains(sample.Id) && 
            sample.Resources?.Any(resource => resource.Type == DataTypes.Omics.Rnasc.Expression) == true
        );
    }
}

